using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NextParkAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ? Servi�os
builder.Services.AddControllers();
builder.Services.AddDbContext<NextParkContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));
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

builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// ? Pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); 

app.Run();
