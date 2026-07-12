-- =============================================================================
-- RoadControl — Nova role GasStationAdmin + vínculo usuário↔posto — 2026-07-12
-- =============================================================================
-- 1) Insere a role "GasStationAdmin" em rc.Roles.
--    IMPORTANTE: o frontend mapeia papéis por Id assumindo a ordem do IDENTITY.
--    Esta role DEVE ficar com Id = 5 (as roles 1–4 são, nesta ordem:
--    SystemAdmin, OrganizationAdmin, Driver, GasStationAttendant).
--    Se o IDENTITY atual da tabela não estiver em 4, ajuste antes de rodar.
-- 2) Adiciona GasStationId (opcional) em rc.Users, com FK para rc.GasStations —
--    usado pelos papéis de posto (GasStationAdmin, GasStationAttendant).
--
-- RECOMENDADO: faça backup antes.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- rc.Roles — nova role GasStationAdmin (Id esperado: 5)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM rc.Roles WHERE Name = 'GasStationAdmin')
BEGIN
    INSERT INTO rc.Roles (Name, Description, CreatedAt, UpdatedAt)
    VALUES ('GasStationAdmin', N'Administrador do posto parceiro — gerencia a equipe do próprio posto',
            SYSUTCDATETIME(), SYSUTCDATETIME());
END
GO

-- Confirmação: o Id gerado deve ser 5 (contrato com o frontend)
SELECT Id, Name FROM rc.Roles WHERE Name = 'GasStationAdmin';
GO

-- -----------------------------------------------------------------------------
-- rc.Users — GasStationId opcional (permite NULL)
-- -----------------------------------------------------------------------------
ALTER TABLE rc.Users ADD GasStationId BIGINT NULL;
GO

ALTER TABLE rc.Users ADD CONSTRAINT FK_Users_GasStations
    FOREIGN KEY (GasStationId) REFERENCES rc.GasStations (Id)
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX IX_Users_GasStationId ON rc.Users (GasStationId);
GO
