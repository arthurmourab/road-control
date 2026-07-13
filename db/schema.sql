-- =============================================================================
-- RoadControl — Schema do banco de dados (SQL Server)
-- =============================================================================
-- Representação canônica do schema REAL do banco, validada por introspecção
-- após a correção de drift aplicada em 2026-05-29
-- (ver db/changes/2026-05-29-corrige-drift-schema.sql).
--
-- O projeto NÃO usa EF Core Migrations: as alterações de schema são aplicadas
-- manualmente no banco. Mantenha este arquivo sincronizado sempre que alterar
-- uma entidade (RC.Domain/Entities) ou mapping (RC.Data/Mappings).
--
-- Todas as tabelas vivem no schema "rc".
-- =============================================================================

-- Schema --------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'rc')
    EXEC('CREATE SCHEMA rc');
GO

-- rc.Roles ------------------------------------------------------------------
CREATE TABLE rc.Roles
(
    Id          BIGINT          NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(100)   NOT NULL,
    Description NVARCHAR(255)   NOT NULL,
    CreatedAt   DATETIME2       NOT NULL,
    UpdatedAt   DATETIME2       NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);
GO

-- rc.Users ------------------------------------------------------------------
CREATE TABLE rc.Users
(
    Id           BIGINT         NOT NULL IDENTITY(1,1),
    Name         NVARCHAR(100)  NOT NULL,
    LastName     NVARCHAR(100)  NOT NULL,
    Email        NVARCHAR(255)  NOT NULL,
    PasswordHash NVARCHAR(500)  NOT NULL,
    RoleId       BIGINT         NOT NULL,
    IsActive     BIT            NOT NULL,
    CreatedAt    DATETIME2      NOT NULL,
    UpdatedAt    DATETIME2      NOT NULL,
    OrganizationId BIGINT       NULL,   -- opcional: papéis de organização (OrganizationAdmin, Driver)
    GasStationId BIGINT         NULL,   -- opcional: papéis de posto (GasStationAdmin, GasStationAttendant)
    ConfirmationSecret NVARCHAR(100) NULL, -- opcional: segredo TOTP do frentista (hex); nunca exposto
    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES rc.Roles (Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Users_Organizations FOREIGN KEY (OrganizationId) REFERENCES rc.Organizations (Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Users_GasStations FOREIGN KEY (GasStationId) REFERENCES rc.GasStations (Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Users_Email ON rc.Users (Email);
GO

CREATE INDEX IX_Users_RoleId ON rc.Users (RoleId);
GO

CREATE INDEX IX_Users_OrganizationId ON rc.Users (OrganizationId);
GO

CREATE INDEX IX_Users_GasStationId ON rc.Users (GasStationId);
GO

-- rc.Organizations ----------------------------------------------------------
CREATE TABLE rc.Organizations
(
    Id        BIGINT          NOT NULL IDENTITY(1,1),
    Name      NVARCHAR(255)   NOT NULL,
    Document  NVARCHAR(14)    NOT NULL,
    IsActive  BIT             NOT NULL,
    CreatedAt DATETIME2       NOT NULL,
    UpdatedAt DATETIME2       NOT NULL,
    CONSTRAINT PK_Organizations PRIMARY KEY (Id)
);
GO

CREATE UNIQUE INDEX UX_Organizations_Document ON rc.Organizations (Document);
GO

-- rc.Vehicles ---------------------------------------------------------------
-- Type é o enum VehicleTypeEnum persistido como INT (HasConversion<int>).
CREATE TABLE rc.Vehicles
(
    Id              BIGINT        NOT NULL IDENTITY(1,1),
    Type            INT           NOT NULL,
    Plate           NVARCHAR(10)  NOT NULL,
    Brand           NVARCHAR(100) NOT NULL,
    Model           NVARCHAR(100) NOT NULL,
    YearManufacture INT           NOT NULL,
    YearModel       INT           NOT NULL,
    Mileage         INT           NOT NULL,
    IsActive        BIT           NOT NULL,
    CreatedAt       DATETIME2     NOT NULL,
    UpdatedAt       DATETIME2     NOT NULL,
    OrganizationId  BIGINT        NOT NULL,   -- obrigatório: todo veículo pertence a uma organização
    CONSTRAINT PK_Vehicles PRIMARY KEY (Id),
    CONSTRAINT FK_Vehicles_Organizations FOREIGN KEY (OrganizationId) REFERENCES rc.Organizations (Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Vehicles_Plate ON rc.Vehicles (Plate);
GO

CREATE INDEX IX_Vehicles_OrganizationId ON rc.Vehicles (OrganizationId);
GO

-- rc.GasStations ------------------------------------------------------------
-- IsGlobal = 1: parceiro global (disponível a todas as organizações).
-- IsGlobal = 0: disponível apenas às organizações em rc.OrganizationGasStations.
CREATE TABLE rc.GasStations
(
    Id           BIGINT        NOT NULL IDENTITY(1,1),
    Name         NVARCHAR(255) NOT NULL,
    Document     NVARCHAR(14)  NOT NULL,
    IsGlobal     BIT           NOT NULL,
    IsActive     BIT           NOT NULL,
    Street       NVARCHAR(255) NOT NULL,
    Number       NVARCHAR(20)  NOT NULL,
    Neighborhood NVARCHAR(100) NOT NULL,
    City         NVARCHAR(100) NOT NULL,
    State        NVARCHAR(2)   NOT NULL,
    ZipCode      NVARCHAR(8)   NOT NULL,
    CreatedAt    DATETIME2     NOT NULL,
    UpdatedAt    DATETIME2     NOT NULL,
    CONSTRAINT PK_GasStations PRIMARY KEY (Id)
);
GO

CREATE UNIQUE INDEX UX_GasStations_Document ON rc.GasStations (Document);
GO

-- rc.OrganizationGasStations ------------------------------------------------
-- Associação N:N entre organizações e postos não-globais.
CREATE TABLE rc.OrganizationGasStations
(
    Id             BIGINT    NOT NULL IDENTITY(1,1),
    OrganizationId BIGINT    NOT NULL,
    GasStationId   BIGINT    NOT NULL,
    CreatedAt      DATETIME2 NOT NULL,
    UpdatedAt      DATETIME2 NOT NULL,
    CONSTRAINT PK_OrganizationGasStations PRIMARY KEY (Id),
    CONSTRAINT FK_OrganizationGasStations_Organizations FOREIGN KEY (OrganizationId)
        REFERENCES rc.Organizations (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_OrganizationGasStations_GasStations FOREIGN KEY (GasStationId)
        REFERENCES rc.GasStations (Id) ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_OrganizationGasStations_Org_Station
    ON rc.OrganizationGasStations (OrganizationId, GasStationId);
GO

CREATE INDEX IX_OrganizationGasStations_GasStationId
    ON rc.OrganizationGasStations (GasStationId);
GO

-- rc.Fuelings ---------------------------------------------------------------
-- Evento central: vincula veículo + posto + motorista + organização.
-- FuelType é o enum FuelTypeEnum persistido como INT.
-- OrganizationId e TotalAmount são snapshots (valor/organização no momento).
CREATE TABLE rc.Fuelings
(
    Id             BIGINT        NOT NULL IDENTITY(1,1),
    VehicleId      BIGINT        NOT NULL,
    GasStationId   BIGINT        NOT NULL,
    DriverId       BIGINT        NOT NULL,
    AttendantId    BIGINT        NULL,   -- frentista que forneceu o código (NULL só em registros históricos)
    OrganizationId BIGINT        NOT NULL,
    FuelType       INT           NOT NULL,
    Liters         DECIMAL(9,3)  NOT NULL,
    PricePerLiter  DECIMAL(18,3) NOT NULL,
    TotalAmount    DECIMAL(18,2) NOT NULL,
    Mileage        INT           NOT NULL,
    FueledAt       DATETIME2     NOT NULL,
    CreatedAt      DATETIME2     NOT NULL,
    UpdatedAt      DATETIME2     NOT NULL,
    CONSTRAINT PK_Fuelings PRIMARY KEY (Id),
    CONSTRAINT FK_Fuelings_Vehicles FOREIGN KEY (VehicleId)
        REFERENCES rc.Vehicles (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Fuelings_GasStations FOREIGN KEY (GasStationId)
        REFERENCES rc.GasStations (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Fuelings_Users FOREIGN KEY (DriverId)
        REFERENCES rc.Users (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Fuelings_Attendants FOREIGN KEY (AttendantId)
        REFERENCES rc.Users (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Fuelings_Organizations FOREIGN KEY (OrganizationId)
        REFERENCES rc.Organizations (Id) ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

CREATE INDEX IX_Fuelings_VehicleId ON rc.Fuelings (VehicleId);
GO
CREATE INDEX IX_Fuelings_GasStationId ON rc.Fuelings (GasStationId);
GO
CREATE INDEX IX_Fuelings_DriverId ON rc.Fuelings (DriverId);
GO
CREATE INDEX IX_Fuelings_AttendantId ON rc.Fuelings (AttendantId);
GO
CREATE INDEX IX_Fuelings_OrganizationId ON rc.Fuelings (OrganizationId);
GO
