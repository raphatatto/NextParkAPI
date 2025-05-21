using Microsoft.EntityFrameworkCore;
using NextParkAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ? Servi�os
builder.Services.AddControllers();
builder.Services.AddDbContext<NextParkContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ? Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); 

app.Run();
