-- =============================================================================
-- RoadControl — Correção de drift de schema — 2026-05-29
-- =============================================================================
-- Alinha o banco ao modelo de domínio. Cobre os itens 1, 2, 3 e 5 levantados
-- na comparação banco x modelo EF.
--
-- RECOMENDADO: faça backup antes e rode em ambiente controlado.
-- Os itens 2 e 1 têm verificações prévias — leia os comentários antes de rodar.
-- =============================================================================

-- =============================================================================
-- ITEM 1 — Organizations: adicionar PRIMARY KEY + IDENTITY no Id
-- -----------------------------------------------------------------------------
-- O SQL Server NÃO permite adicionar IDENTITY a uma coluna existente via
-- ALTER COLUMN, então é necessário recriar a tabela. Organizations não é
-- referenciada por nenhuma FK, então o rebuild é seguro.
--
-- ATENÇÃO: se a tabela tiver linhas com Id = 0 (provável efeito do bug, já que
-- o banco não auto-gerava o Id), elas serão preservadas como 0. Avalie se vale
-- limpar/recadastrar esses registros ANTES de rodar este bloco.
-- =============================================================================
BEGIN TRANSACTION;

CREATE TABLE rc.Organizations_new
(
    Id        BIGINT        NOT NULL IDENTITY(1,1),
    Name      NVARCHAR(255) NOT NULL,
    Document  NVARCHAR(14)  NOT NULL,
    IsActive  BIT           NOT NULL,
    CreatedAt DATETIME2     NOT NULL,
    UpdatedAt DATETIME2     NOT NULL,
    CONSTRAINT PK_Organizations PRIMARY KEY (Id)
);

-- Copia os dados preservando os Ids existentes
SET IDENTITY_INSERT rc.Organizations_new ON;
INSERT INTO rc.Organizations_new (Id, Name, Document, IsActive, CreatedAt, UpdatedAt)
SELECT Id, Name, Document, IsActive, CreatedAt, UpdatedAt
FROM rc.Organizations;
SET IDENTITY_INSERT rc.Organizations_new OFF;

-- Substitui a tabela antiga (o índice UX_Organizations_Document cai junto)
DROP TABLE rc.Organizations;
EXEC sp_rename 'rc.Organizations_new', 'Organizations';

-- Recria o índice único de Document
CREATE UNIQUE INDEX UX_Organizations_Document ON rc.Organizations (Document);

COMMIT;
GO

-- =============================================================================
-- ITEM 2 — Vehicles.Plate: criar índice único
-- -----------------------------------------------------------------------------
-- VERIFIQUE DUPLICATAS PRIMEIRO. Se a consulta abaixo retornar linhas, o
-- CREATE UNIQUE INDEX vai falhar — resolva as placas duplicadas antes.
--   SELECT Plate, COUNT(*) FROM rc.Vehicles GROUP BY Plate HAVING COUNT(*) > 1;
-- =============================================================================
CREATE UNIQUE INDEX UX_Vehicles_Plate ON rc.Vehicles (Plate);
GO

-- =============================================================================
-- ITEM 3 — Vehicles: timestamps DATETIME -> DATETIME2 (alinha com as demais tabelas)
-- =============================================================================
ALTER TABLE rc.Vehicles ALTER COLUMN CreatedAt DATETIME2 NOT NULL;
ALTER TABLE rc.Vehicles ALTER COLUMN UpdatedAt DATETIME2 NOT NULL;
GO

-- =============================================================================
-- ITEM 5 — UpdatedAt NOT NULL uniforme (Roles e Users hoje permitem NULL)
-- -----------------------------------------------------------------------------
-- Preenche eventuais NULLs com CreatedAt antes de tornar a coluna NOT NULL.
-- =============================================================================
UPDATE rc.Roles SET UpdatedAt = CreatedAt WHERE UpdatedAt IS NULL;
ALTER TABLE rc.Roles ALTER COLUMN UpdatedAt DATETIME2 NOT NULL;

UPDATE rc.Users SET UpdatedAt = CreatedAt WHERE UpdatedAt IS NULL;
ALTER TABLE rc.Users ALTER COLUMN UpdatedAt DATETIME2 NOT NULL;
GO
