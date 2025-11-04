// tests/NextParkAPI.Tests/Unit/ManutencaoControllerTests.cs
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

namespace MinhaAPITeste
{
    public class ManutencaoControllerTests
    {
        private NextParkContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<NextParkContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var ctx = new NextParkContext(options);

            // Semear uma moto, pq o controller exige que a moto exista
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

            // mock do UrlHelper pra não quebrar na hora de montar links
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                   .Returns("/api/v1.0/Manutencao");
            controller.Url = mockUrl.Object;

            var httpContext = new DefaultHttpContext();
            var apiVersion = new ApiVersion(1, 0);
            // a classe ApiVersioningFeature PRECISA do HttpContext no construtor:
            httpContext.Features.Set<IApiVersioningFeature>(
                new ApiVersioningFeature(httpContext) { RequestedApiVersion = apiVersion }
            );
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        [Fact]
        public async Task GetManutencoes_Deve_retornar_BadRequest_quando_paginacao_invalida()
        {
            // arrange
            var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            // act
            var result = await controller.GetManutencoes(0, 10);

            // assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetManutencoes_Deve_retornar_pagina_com_itens()
        {
            var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = await controller.GetManutencoes(1, 10);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();

            dynamic response = ok!.Value!;
            ((int)response.TotalCount).Should().Be(2);
        }

        [Fact]
        public async Task GetManutencao_Deve_retornar_NotFound_quando_nao_existe()
        {
            var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = await controller.GetManutencao(999);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateManutencao_Deve_retornar_BadRequest_quando_moto_nao_existe()
        {
            var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var nova = new Manutencao { IdMoto = 999, DsManutencao = "Pastilha" }; 

            var result = await controller.CreateManutencao(nova);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateManutencao_Deve_retornar_Created_quando_valido()
        {
            var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var nova = new Manutencao
            {
                IdMoto = 1,
                DsManutencao = "Pastilha de freio"
            };

            var result = await controller.CreateManutencao(nova);

            var created = result.Result as CreatedAtActionResult;
            created.Should().NotBeNull();
            created!.StatusCode.Should().Be(201);

            // e foi salvo no banco
            ctx.Manutencoes.Count().Should().Be(3);
        }
    }
}
