using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ? Serviços
builder.Services.AddControllers();
builder.Services.AddDbContext<NextParkContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
