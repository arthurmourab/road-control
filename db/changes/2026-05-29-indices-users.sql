-- =============================================================================
-- RoadControl — Índices em rc.Users — 2026-05-29
-- =============================================================================
-- Trata duas lacunas conhecidas:
--   - Users.Email sem índice único (unicidade era checada só em código)
--   - Users.RoleId sem índice (apenas a FK existia)
--
-- RECOMENDADO: faça backup antes.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- VERIFIQUE DUPLICATAS DE EMAIL PRIMEIRO.
-- Se a consulta abaixo retornar linhas, o CREATE UNIQUE INDEX vai falhar —
-- resolva os emails duplicados antes de criar o índice.
--   SELECT Email, COUNT(*) FROM rc.Users GROUP BY Email HAVING COUNT(*) > 1;
-- -----------------------------------------------------------------------------

-- Índice único em Email
CREATE UNIQUE INDEX UX_Users_Email ON rc.Users (Email);
GO

-- Índice (não-único) na coluna da FK RoleId
CREATE INDEX IX_Users_RoleId ON rc.Users (RoleId);
GO
