using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationParts; // <- IMPORTANTE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextParkAPI;
using NextParkAPI.Controllers; // <- para referenciar ManutencaoController
using NextParkAPI.Data;
using NextParkAPI.Models;

namespace MinhaAPITeste
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove o DbContext real
                var toRemove = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<NextParkContext>) ||
                        d.ServiceType == typeof(NextParkContext) ||
                        d.ServiceType?.FullName == "Microsoft.EntityFrameworkCore.IDbContextFactory`1" ||
                        d.ImplementationType == typeof(NextParkContext))
                    .ToList();
                foreach (var d in toRemove) services.Remove(d);

                // InMemory
                services.AddDbContext<NextParkContext>(o =>
                    o.UseInMemoryDatabase("NextParkAPI_TestDb"));

                // Versionamento por SEGMENTO (igual ao app)
                services.AddApiVersioning(o =>
                {
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.ReportApiVersions = true;
                    o.ApiVersionReader = new UrlSegmentApiVersionReader();
                });
                services.AddVersionedApiExplorer(o =>
                {
                    o.GroupNameFormat = "'v'VVV";
                    o.SubstituteApiVersionInUrl = true;
                });

                // Controllers + força carregar o assembly da API
                services.AddControllers()
                    .PartManager.ApplicationParts.Add(
                        new AssemblyPart(typeof(ManutencaoController).Assembly)
                    );

                // Seed
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<NextParkContext>();
                db.Database.EnsureCreated();

                if (!db.Motos.Any())
                {
                    db.Motos.Add(new Moto
                    {
                        IdMoto = 1,
                        NrPlaca = "INT0001",
                        NmModelo = "Moto integração",
                        IdVaga = 1,
                        StMoto = 'L'
                    });
                }

                if (!db.Manutencoes.Any())
                {
                    db.Manutencoes.Add(new Manutencao
                    {
                        IdManutencao = 1,
                        IdMoto = 1,
                        DsManutencao = "Manutenção inicial"
                    });
                }

                db.SaveChanges();
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });
        }
    }
}
