-- =============================================================================
-- RoadControl — Criação do módulo de Postos — 2026-05-29
-- =============================================================================
-- Cria as tabelas do módulo de postos parceiros:
--   - rc.GasStations            : os postos (parceiros globais ou vinculados)
--   - rc.OrganizationGasStations: associação N:N posto <-> organização
--                                 (usada apenas para postos não-globais)
--
-- RECOMENDADO: faça backup antes.
-- =============================================================================

-- rc.GasStations ------------------------------------------------------------
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

-- rc.OrganizationGasStations (associação N:N) -------------------------------
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

-- Impede vincular a mesma organização ao mesmo posto mais de uma vez
CREATE UNIQUE INDEX UX_OrganizationGasStations_Org_Station
    ON rc.OrganizationGasStations (OrganizationId, GasStationId);
GO

-- Índice de apoio para a FK de GasStation
CREATE INDEX IX_OrganizationGasStations_GasStationId
    ON rc.OrganizationGasStations (GasStationId);
GO
