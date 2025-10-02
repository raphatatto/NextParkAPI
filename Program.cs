using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NextParkAPI.Data;
using Microsoft.AspNetCore.HttpOverrides;
using NextParkAPI.Utils;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<NextParkContext>(options =>
{
    var env = builder.Environment.EnvironmentName;
    if (env == "Production")
    {
        // usa o Azure SQL em produção
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        // continua usando Oracle no desenvolvimento
        options.UseOracle(builder.Configuration.GetConnectionString("OracleDb"));
    }
});

builder.Services.AddScoped<IPrimaryKeyGenerator, OraclePrimaryKeyGenerator>();
builder.Services.AddScoped<IPrimaryKeyGenerator, SqlServerPrimaryKeyGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NextPark API",
        Version = "v1",
        Description = "API para gestão de motos, vagas e manutenções em um pátio de estacionamento, incluindo paginação e hipermídia.",
        Contact = new OpenApiContact
        {
            Name = "NextPark",
            Email = "contato@nextpark.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    options.EnableAnnotations();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});
var port = Environment.GetEnvironmentVariable("WEBSITES_PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Migração só em prod (como você já fez)
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<NextParkContext>();
    db.Database.Migrate();
}

// Forwarded headers (Azure) — opção por código
var fwd = new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
};
fwd.KnownNetworks.Clear();
fwd.KnownProxies.Clear();
app.UseForwardedHeaders(fwd);

// HTTPS redirect (só em prod, se preferir)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
