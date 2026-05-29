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
    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES rc.Roles (Id)
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Users_Email ON rc.Users (Email);
GO

CREATE INDEX IX_Users_RoleId ON rc.Users (RoleId);
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
    CONSTRAINT PK_Vehicles PRIMARY KEY (Id)
);
GO

CREATE UNIQUE INDEX UX_Vehicles_Plate ON rc.Vehicles (Plate);
GO
