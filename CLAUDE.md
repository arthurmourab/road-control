# RoadControl — Documento de suporte a agentes de IA

## Objetivo
Este documento serve de referência de contexto e de padrão arquitetural para o sistema
RoadControl (RC). Agentes de IA que leiam ou alterem o código de qualquer maneira devem
consultar este documento **antes** de tomar suas decisões, e manter a consistência com os
padrões aqui descritos.

## O que o sistema é
RoadControl é um software em desenvolvimento que auxilia organizações no controle de suas
frotas de automóveis, mais especificamente no controle do abastecimento desses automóveis.
O sistema atua como uma ponte entre as organizações e postos de combustível parceiros. O
motorista de um veículo de uma organização que utilize o RC sempre deverá abastecer em um
posto parceiro e registrar no app os dados desse abastecimento para controle da organização.

---

## Arquitetura e camadas

API .NET (net10.0) em arquitetura em camadas. A solução é `RoadControl.slnx`. A dependência
entre projetos é **unidirecional** — uma camada só conhece as que estão acima dela na tabela:

| Projeto        | Responsabilidade                                                        | Depende de        |
|----------------|-------------------------------------------------------------------------|-------------------|
| **RC.Domain**  | Entidades, interfaces (repos e services), exceções de domínio           | (nada)            |
| **RC.Shared**  | Contratos: DTOs, enums, `ApiResponse<T>`, `PagedResult<T>`              | (nada)            |
| **RC.Data**    | EF Core: `RCDbContext`, mappings, implementações de repositórios        | Domain, Shared    |
| **RC.Service** | Regras de negócio, mapeamento DTO↔Entity, registro de DI                | Domain, Data, Shared |
| **RC.WebApi**  | Controllers, pipeline HTTP, middleware, autenticação                    | todas             |

Regras de dependência:
- **Domain não depende de ninguém.** Não importe EF Core, ASP.NET ou Shared dentro de Domain.
- **Service depende da _interface_ do repositório (`IXRepository`), nunca da implementação concreta.**
- **Controller nunca acessa repositório nem `DbContext` direto** — só fala com o service.

---

## Fluxo de uma requisição

```
Controller  →  IService  →  IRepository  →  RCDbContext (EF Core)
   (fino)      (regra)       (acesso a dados)
```

1. O **controller** recebe o DTO, chama o service e embrulha o retorno em `ApiResponse<T>`.
2. O **service** valida regras, mapeia DTO↔Entity e orquestra os repositórios.
3. O **repositório** executa as queries via `RCDbContext`.

---

## Convenções de código

- **Idioma:** comentários e mensagens em **português**. Identificadores (classes, métodos,
  variáveis) em **inglês**.
- **Injeção de dependência:** use **primary constructors** e atribua a um campo privado.
  ```csharp
  public class OrganizationService(IOrganizationRepository organizationRepository) : IOrganizationService
  {
      private readonly IOrganizationRepository _organizationRepository = organizationRepository;
  }
  ```
- **Registro de DI:** todo service e repositório novo é registrado como `Scoped` em
  `RC.Service/Extensions/ServiceCollectionExtensions.cs` (`AddServices`). Não registre no `Program.cs`.
- **DTOs:** entrada de criação usa o prefixo `New` (`NewOrganizationDto`); entrada de atualização
  usa o prefixo `Update` (`UpdateVehicleDto`, `UpdateStatusDto`); leitura/saída usa o nome simples
  (`OrganizationDto`). Entidades **nunca** são expostas direto pela API.
- **Mapeamento é manual** (sem AutoMapper), feito por métodos privados no service:
  `MapXToXDto`, `MapNewXDtoToX`, e a versão de lista. Mantenha esse padrão.
- **Async em toda I/O:** métodos de repositório/service terminam em `Async` e retornam `Task`.
- **Auditoria:** toda entidade herda de `BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`).
  `CreatedAt`/`UpdatedAt` são preenchidos automaticamente no override de `SaveChangesAsync` —
  **não defina essas datas manualmente.**
- **Paginação:** listagens retornam `PagedResult<T>` e o repositório expõe um `GetAll...Async`
  paginado + um `GetAllTotalAsync` para a contagem.

---

## Respostas e tratamento de erros

- **Toda resposta da API é embrulhada em `ApiResponse<T>`** — nunca retorne a entidade/DTO cru.
  - Sucesso: `ApiResponse<T>.Ok(data)`.
  - Falha: `ApiResponse<object>.Fail(mensagem)` (usado pelo middleware e pela validação de model).
- **Erros são sinalizados por exceções de domínio**, não por códigos de retorno. O service
  lança; o controller **não tem try/catch**.
  - `NotFoundException` → 404
  - `ConflictException` → 409
  - `BusinessRuleException` → 422
  - `UnauthorizedAccessException` → 401
  - O `ExceptionHandlingMiddleware` (em `RC.WebApi/Middleware`) converte a exceção na resposta
    HTTP padronizada. Ao criar um novo tipo de erro, crie a exceção em `RC.Domain/Exceptions`
    e trate-a nesse middleware.
- **Validação de DTO** é feita por DataAnnotations; falhas retornam 400 já no formato
  `ApiResponse` (configurado em `Program.cs` via `InvalidModelStateResponseFactory`).

---

## Autenticação e autorização

- Autenticação via **JWT Bearer** (configuração na seção `Jwt` do `appsettings`).
- Os papéis ficam centralizados em constantes em `Role.Roles`
  (`SystemAdmin`, `OrganizationAdmin`, `Driver`, `GasStationAttendant`). Combinações de papéis
  também viram constantes ali (`FuelingManagers`, `UserManagers`) — ao autorizar um grupo de
  papéis, crie/reuse a constante composta em vez de concatenar strings no controller.
- Proteja endpoints com `[Authorize(Roles = Role.Roles.SystemAdmin)]` — **sempre via a
  constante**, nunca com string literal.
- **Isolamento por organização:** usuários pertencem a uma organização (`User.OrganizationId`,
  nulo para SystemAdmin). O padrão é: o controller extrai o chamador do JWT via
  `User.GetUserId()` / `User.GetRole()` (`RC.WebApi/Extensions/ClaimsPrincipalExtensions.cs`)
  e repassa ao service, que **restringe a consulta/operação à organização do chamador**.
  SystemAdmin enxerga tudo e pode filtrar via query string `?organizationId=`. Todo endpoint
  novo que toque dados de organização deve seguir esse padrão.
- **CORS:** política `Frontend` no `Program.cs` — em desenvolvimento libera qualquer origem
  loopback; em produção libera apenas as origens da seção `Cors:AllowedOrigins` do `appsettings`.

---

## Convenções da Web API

- Rotas: `[Route("/v1/[controller]")]`. URLs são forçadas a **minúsculas**.
- Enums são serializados como **string** no JSON.
- Use o status code explícito coerente com a semântica (`201` para criação, `200` para leitura).

---

## Checklist: como adicionar um novo módulo

Ao criar um módulo novo (ex.: "Posto", "Abastecimento"), siga esta ordem para respeitar as camadas:

1. **Domain** — criar a entidade em `RC.Domain/Entities` herdando de `BaseEntity`.
2. **Domain** — criar a interface do repositório (`IXRepository`) e a do service (`IXService`)
   nas respectivas pastas de `Interfaces`.
3. **Shared** — criar os DTOs `NewXDto` (entrada) e `XDto` (saída) em `RC.Shared/Dtos/X`.
4. **Data** — criar o mapping em `RC.Data/Mappings` e a implementação `XRepository`.
5. **Data** — registrar o `DbSet`/mapping no `RCDbContext` se necessário.
6. **Service** — implementar `XService` com as regras e os métodos de mapeamento manuais.
7. **DI** — registrar service e repositório em `ServiceCollectionExtensions.AddServices`.
8. **WebApi** — criar o `XController` fino, embrulhando respostas em `ApiResponse<T>` e
   aplicando `[Authorize]` com os papéis adequados.

---

## Comandos úteis

```bash
dotnet build RoadControl.slnx     # compilar a solução
dotnet run --project RC.WebApi    # subir a API (Swagger abre em ambiente Development)
```

> O banco usa SQL Server (`DefaultConnection`). O Swagger só é exposto em ambiente de
> desenvolvimento.

> **Não use migrations.** Este projeto não usa EF Core Migrations — as alterações de schema
> são aplicadas manualmente no banco. Ao criar ou alterar uma entidade, atualize a entidade,
> o mapping e o `RCDbContext` se necessário, mas **não** gere migrations
> (`dotnet ef migrations ...`).
>
> O schema canônico do banco fica em [`db/schema.sql`](db/schema.sql) (DDL completo, schema
> `rc`). **Mantenha-o sincronizado** sempre que alterar uma entidade ou mapping. Alterações
> incrementais de schema são registradas como scripts datados em `db/changes/`
> (ex.: `2026-05-29-cria-postos.sql`) — ao alterar o schema, crie o script da mudança ali
> **e** atualize o `schema.sql`.
