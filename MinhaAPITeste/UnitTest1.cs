using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Moq;
using NextParkAPI.Controllers;
using NextParkAPI.Data;
using NextParkAPI.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
namespace MinhaAPITeste
{
    public class UnitTest1
    {
        private NextParkContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<NextParkContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var ctx = new NextParkContext(options);

            ctx.Motos.Add(new Moto { IdMoto = 1, NmModelo = "Tiger test", IdVaga = 1, NrPlaca = "ABC1234", StMoto = 'L' });
            ctx.Manutencoes.AddRange(
                new Manutencao { IdManutencao = 1, IdMoto = 1, DsManutencao = "Troca de óleo" },
                new Manutencao { IdManutencao = 2, IdMoto = 1, DsManutencao = "Revisão" }
            );
            ctx.SaveChanges();
            return ctx;
        }

 

private ManutencaoController CreateController(NextParkContext ctx)
    {
        var controller = new ManutencaoController(ctx);

        var mockUrl = new Mock<IUrlHelper>();
        mockUrl.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
               .Returns("/api/v1.0/Manutencao");
        controller.Url = mockUrl.Object;

        var httpContext = new DefaultHttpContext();

        // Simula a feature de versionamento
        var apiVersion = new ApiVersion(1, 0);
            httpContext.Features.Set<IApiVersioningFeature>(
            new ApiVersioningFeature(httpContext) { RequestedApiVersion = apiVersion }
            );


            controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }

    [Fact]
        public async Task GetManutencoes_quando_pagina_invalida_retorna_badrequest()
        {
            var ctx = CreateContext();
            var ctrl = CreateController(ctx);

            var result = await ctrl.GetManutencoes(0, 10);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetManutencoes_retorna_lista()
        {
            var ctx = CreateContext();
            var ctrl = CreateController(ctx);

            var result = await ctrl.GetManutencoes(1, 10);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();

            dynamic response = ok!.Value!;
            ((int)response.TotalCount).Should().Be(2);
        }

        [Fact]
        public async Task GetManutencao_inexistente_retorna_404()
        {
            var ctx = CreateContext();
            var ctrl = CreateController(ctx);

            var result = await ctrl.GetManutencao(999);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateManutencao_com_moto_inexistente_retorna_badrequest()
        {
            var ctx = CreateContext();
            var ctrl = CreateController(ctx);

            var nova = new Manutencao { IdMoto = 999, DsManutencao = "x" };

            var result = await ctrl.CreateManutencao(nova);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateManutencao_valida_cria_e_retorna_201()
        {
            var ctx = CreateContext();
            var ctrl = CreateController(ctx);

            var nova = new Manutencao { IdMoto = 1, DsManutencao = "Pastilha" };

            var result = await ctrl.CreateManutencao(nova);

            var created = result.Result as CreatedAtActionResult;
            created.Should().NotBeNull();
            ctx.Manutencoes.Count().Should().Be(3);
        }
    }
}
