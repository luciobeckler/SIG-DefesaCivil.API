using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIG_DefesaCivil.API.Data;
using SIG_DefesaCivil.API.Data.Context;
using SIG_DefesaCivil.API.Helper;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;
using SIG_DefesaCivil.API.TokenGenerator;
using SIG_DefesaCivil.API.Workers;
using System.Text;
using System.Text.Json.Serialization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new SIG_DefesaCivil.API.Helper.DateTimeUtcConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Adicione esta linha:
    c.SchemaFilter<SIG_DefesaCivil.API.Helper.EnumListSchemaFilter>();
});

string connectionName;

if (builder.Environment.IsDevelopment())
{
    connectionName = "DevConnection";
}
else if (builder.Environment.IsStaging())
{
    connectionName = "StagingConnection";
}
else
{
    connectionName = "ProdConnection";
}

var connectionString = builder.Configuration.GetConnectionString(connectionName);

builder.Services.AddDbContext<DefesaCivilDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registrando services
builder.Services.AddScoped<NaturezaService>();
builder.Services.AddScoped<OcorrenciaService>();
builder.Services.AddScoped<QuadroService>();
builder.Services.AddScoped<EtapaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AnexoService>();
builder.Services.AddHostedService<AutomacaoMovimentacaoWorker>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddSingleton<DriveService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var clientSecrets = new ClientSecrets
    {
        ClientId = config["GoogleDrive:ClientId"],
        ClientSecret = config["GoogleDrive:ClientSecret"]
    };

    var token = new TokenResponse
    {
        RefreshToken = config["GoogleDrive:RefreshToken"]
    };

    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
    {
        ClientSecrets = clientSecrets,
        Scopes = new[] { DriveService.Scope.Drive }
    });

    var credential = new UserCredential(flow, "user", token);

    return new DriveService(new BaseClientService.Initializer()
    {
        HttpClientInitializer = credential,
        ApplicationName = "SIG-DefesaCivil-API"
    });
});

// 2. Registrar seu wrapper como Scoped (Um por requisição)
builder.Services.AddScoped<GoogleDriveService>();

//Configurando características da senha
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
}).AddEntityFrameworkStores<DefesaCivilDbContext>()
  .AddDefaultTokenProviders();

// Configurando autenticação de cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Configurando características do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida se quem assinou foi este servidor mesmo
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

        // Validação de Emissor (Issuer) e Publico (Audience)
        // DICA: Em desenvolvimento, as vezes é útil deixar false se estiver tendo problemas com URLs
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,

        // Certifique-se que no appsettings.json estes valores batem EXATAMENTE 
        // com o que está sendo gerado no TokenService
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Tolerância de tempo para clocks desincronizados (opcional, padrão é 5min)
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    // REMOVIDO: options.Events = new JwtBearerEvents { ... }
    // Agora o .NET vai buscar automaticamente no Header "Authorization: Bearer <token>"
});

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8100",
                "http://localhost:4200",
                "URL DE PRODUÇÃO")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("StaggingPolicy", policy =>
    {
        policy.AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true);
    });
});

// Fazendo com que todos os end-points exijam autenticação por padrão
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Services e repositorys
builder.Services.AddScoped<TokenService>();
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});

var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseCors("LocalPolicy");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (builder.Environment.IsStaging())
{
    app.UseCors("StaggingPolicy");
}
else
{
    app.UseCors("StaggingPolicy");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Chamada única que resolve tudo
        await Seeder.SeedAllAsync(services);

        // Log de sucesso opcional
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Banco de dados populado com sucesso (Roles, Users, Quadros, Naturezas).");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao popular o banco de dados.");
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DefesaCivilDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrations.");
    }
}

app.Run();