using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIG_DefesaCivil.API.Context;
using SIG_DefesaCivil.API.Models;
using SIG_DefesaCivil.API.Services;
using SIG_DefesaCivil.API.TokenGenerator;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurando banco de dados com Entity Framework Core e SQL Server
var connectionName = builder.Environment.IsDevelopment() ? "DevConnection" : "ProdConnection";
var connectionString = builder.Configuration.GetConnectionString(connectionName);

builder.Services.AddDbContext<DefesaCivilDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrando services
builder.Services.AddScoped<NaturezaService>();

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
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["auth_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

    // Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8100",
                "http://localhost:4200",
                "URL DE PRODUÇÃO")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
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
builder.Services.AddScoped<JwtTokenGenerator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

    var roles = new[] { "Administrador", "Diretor", "Usuário de campo" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@teste.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        Usuario newAdmin = new Usuario
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nome = "Lúcio Beckler Passos",
            Telefone = "31985211711",
            CPF = "14485403645",
            Cargo = "Administrador do sistema",
            DataAdmissao = DateOnly.FromDateTime(DateTime.Now),
            isAtivo = true,
            isPrimeiroAcesso = false
        };

        var result = await userManager.CreateAsync(newAdmin, "SenhaForte123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(newAdmin, "Administrador");
    }
}

app.Run();
