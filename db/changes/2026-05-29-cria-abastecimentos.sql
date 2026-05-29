-- =============================================================================
-- RoadControl — Criação do módulo de Abastecimentos — 2026-05-29
-- =============================================================================
-- Cria a tabela rc.Fuelings, o evento central do sistema, que vincula
-- veículo + posto + motorista + organização.
--
-- RECOMENDADO: faça backup antes.
-- =============================================================================

CREATE TABLE rc.Fuelings
(
    Id             BIGINT        NOT NULL IDENTITY(1,1),
    VehicleId      BIGINT        NOT NULL,
    GasStationId   BIGINT        NOT NULL,
    DriverId       BIGINT        NOT NULL,   -- usuário (motorista) que registrou
    OrganizationId BIGINT        NOT NULL,   -- snapshot da organização do veículo
    FuelType       INT           NOT NULL,   -- enum FuelTypeEnum
    Liters         DECIMAL(9,3)  NOT NULL,
    PricePerLiter  DECIMAL(18,3) NOT NULL,
    TotalAmount    DECIMAL(18,2) NOT NULL,
    Mileage        INT           NOT NULL,   -- odômetro no momento do abastecimento
    FueledAt       DATETIME2     NOT NULL,   -- data/hora do abastecimento
    CreatedAt      DATETIME2     NOT NULL,
    UpdatedAt      DATETIME2     NOT NULL,
    CONSTRAINT PK_Fuelings PRIMARY KEY (Id),
    CONSTRAINT FK_Fuelings_Vehicles FOREIGN KEY (VehicleId)
        REFERENCES rc.Vehicles (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Fuelings_GasStations FOREIGN KEY (GasStationId)
        REFERENCES rc.GasStations (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Fuelings_Users FOREIGN KEY (DriverId)
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
CREATE INDEX IX_Fuelings_OrganizationId ON rc.Fuelings (OrganizationId);
GO
