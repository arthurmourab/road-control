// Importações de namespaces necessários para configurar autenticação, MVC, JWT, Swagger e serialização
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RC.Service.Extensions;
using RC.Shared.Models.Results;
using RC.WebApi.Middleware;
using System.Text;
using System.Text.Json.Serialization;

// Cria o builder da aplicação, que é responsável por registrar serviços e configurar o pipeline HTTP
var builder = WebApplication.CreateBuilder(args);

// Registra no container de DI todos os serviços e repositórios definidos em ServiceCollectionExtensions
// (DbContext, VehicleService, UserService, AuthService, etc.)
builder.Services.AddServices(builder.Configuration);

// Força todas as URLs de rota a serem geradas em letras minúsculas (ex: /v1/user em vez de /v1/User)
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Registra os controllers da aplicação e encadeia configurações adicionais
builder.Services.AddControllers()
    // Faz com que enums sejam serializados como string no JSON (ex: "Car" em vez de 0)
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    // Sobrescreve o comportamento padrão do [ApiController] ao falhar a validação do model
    .ConfigureApiBehaviorOptions(options =>
    {
        // Quando um DTO não passa nas DataAnnotations, retorna 400 no formato ApiResponse em vez do ProblemDetails padrão do ASP.NET
        options.InvalidModelStateResponseFactory = context =>
        {
            // Extrai a primeira mensagem de erro de validação do ModelState
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .First();

            // Retorna 400 Bad Request com a mensagem de erro no envelope ApiResponse
            return new BadRequestObjectResult(ApiResponse<object>.Fail(errors));
        };
    });

// Configura o Swashbuckle para gerar a documentação OpenAPI (Swagger)
builder.Services.AddSwaggerGen(options =>
{
    // Define os metadados do documento OpenAPI (título e versão exibidos na UI do Swagger)
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RoadControl API", Version = "v1" });

    // Registra o esquema de segurança "Bearer" no documento OpenAPI
    // Isso faz o botão "Authorize" aparecer na UI do Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",         // Nome do header HTTP que será usado
        Type = SecuritySchemeType.Http, // Tipo HTTP (o Swagger adiciona "Bearer " automaticamente)
        Scheme = "bearer",              // Esquema lowercase exigido pela spec OpenAPI
        BearerFormat = "JWT",           // Informativo: indica que o token é um JWT
        In = ParameterLocation.Header   // O token vai no header da requisição
    });

    // Aplica o requisito de segurança Bearer a todas as operações do documento
    // O parâmetro `doc` é o documento OpenAPI gerado, necessário para resolver a referência corretamente
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>()
        }
    });
});

// Lê a seção "Jwt" do appsettings.json (Secret, Issuer, Audience, ExpirationMinutes)
var jwtConfig = builder.Configuration.GetSection("Jwt");

// Registra o serviço de autenticação usando JWT Bearer como esquema padrão
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Define os parâmetros que serão validados em cada token recebido
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,           // Verifica se o campo "iss" do token bate com ValidIssuer
            ValidateAudience = true,         // Verifica se o campo "aud" do token bate com ValidAudience
            ValidateLifetime = true,         // Rejeita tokens expirados (valida o campo "exp")
            ValidateIssuerSigningKey = true, // Verifica a assinatura do token com a chave secreta
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            // Reconstrói a chave simétrica usada para assinar e validar os tokens
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["Secret"]!))
        };
    });

// Constrói a aplicação com todos os serviços registrados acima
var app = builder.Build();

// Adiciona o middleware de autenticação ao pipeline — identifica quem é o usuário (lê e valida o JWT)
app.UseAuthentication();

// Adiciona o middleware de autorização ao pipeline — verifica se o usuário tem permissão para acessar o recurso
// Deve sempre vir após UseAuthentication()
app.UseAuthorization();

// Ativa o Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    // Expõe o endpoint /swagger/v1/swagger.json com o documento OpenAPI gerado
    app.UseSwagger();
    // Serve a interface visual do Swagger UI apontando para o documento gerado
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RoadControl API v1"));
}

// Redireciona automaticamente requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Adiciona o middleware customizado de tratamento de exceções ao pipeline
// Captura exceções lançadas pelos controllers e as converte em respostas HTTP padronizadas
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Mapeia as rotas dos controllers para os endpoints HTTP da aplicação
app.MapControllers();

// Inicia a aplicação e começa a escutar requisições HTTP
app.Run();
