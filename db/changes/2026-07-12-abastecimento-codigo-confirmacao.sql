-- =============================================================================
-- RoadControl — Código de confirmação do abastecimento — 2026-07-12
-- =============================================================================
-- 1) rc.Users.ConfirmationSecret (NULL): segredo (hex) do frentista para derivar
--    o código de confirmação (TOTP). Provisionado sob demanda; nunca exposto.
-- 2) rc.Fuelings.AttendantId (NULL): frentista que forneceu o código.
--    NULLable no banco (registros históricos não têm frentista); em novos
--    registros a aplicação SEMPRE preenche.
--
-- RECOMENDADO: faça backup antes.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- rc.Users — ConfirmationSecret (opcional)
-- -----------------------------------------------------------------------------
ALTER TABLE rc.Users ADD ConfirmationSecret NVARCHAR(100) NULL;
GO

-- -----------------------------------------------------------------------------
-- rc.Fuelings — AttendantId (opcional no DB, sempre preenchido em novos registros)
-- -----------------------------------------------------------------------------
ALTER TABLE rc.Fuelings ADD AttendantId BIGINT NULL;
GO

ALTER TABLE rc.Fuelings ADD CONSTRAINT FK_Fuelings_Attendants
    FOREIGN KEY (AttendantId) REFERENCES rc.Users (Id)
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX IX_Fuelings_AttendantId ON rc.Fuelings (AttendantId);
GO
