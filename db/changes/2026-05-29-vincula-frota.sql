-- =============================================================================
-- RoadControl — Vínculo de frota (Vehicle/User -> Organization) — 2026-05-29
-- =============================================================================
-- Adiciona OrganizationId em:
--   - rc.Vehicles : OBRIGATÓRIO (todo veículo pertence a uma organização)
--   - rc.Users    : OPCIONAL    (SystemAdmin não pertence a nenhuma organização)
--
-- RECOMENDADO: faça backup antes.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- rc.Vehicles — OrganizationId obrigatório
-- -----------------------------------------------------------------------------
-- A coluna é adicionada como NULL para não falhar caso já existam veículos.
-- Se houver dados, ATRIBUA a organização de cada veículo no UPDATE abaixo ANTES
-- de tornar a coluna NOT NULL (troque <ID_ORG> pelo Id da organização correta).
-- Se a tabela estiver vazia, o UPDATE não afeta nada e pode rodar tudo direto.
ALTER TABLE rc.Vehicles ADD OrganizationId BIGINT NULL;
GO

-- UPDATE rc.Vehicles SET OrganizationId = <ID_ORG> WHERE OrganizationId IS NULL;
-- GO

ALTER TABLE rc.Vehicles ALTER COLUMN OrganizationId BIGINT NOT NULL;
GO

ALTER TABLE rc.Vehicles ADD CONSTRAINT FK_Vehicles_Organizations
    FOREIGN KEY (OrganizationId) REFERENCES rc.Organizations (Id)
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX IX_Vehicles_OrganizationId ON rc.Vehicles (OrganizationId);
GO

-- -----------------------------------------------------------------------------
-- rc.Users — OrganizationId opcional (permite NULL)
-- -----------------------------------------------------------------------------
ALTER TABLE rc.Users ADD OrganizationId BIGINT NULL;
GO

ALTER TABLE rc.Users ADD CONSTRAINT FK_Users_Organizations
    FOREIGN KEY (OrganizationId) REFERENCES rc.Organizations (Id)
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX IX_Users_OrganizationId ON rc.Users (OrganizationId);
GO
