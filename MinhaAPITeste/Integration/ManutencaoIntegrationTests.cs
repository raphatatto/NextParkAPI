using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NextParkAPI.Models;
using Xunit;
using MinhaAPITeste; 

namespace MinhaAPITeste.Integration 
{
    public class ManutencaoIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ManutencaoIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
        }

        [Fact]
        public async Task GET_api_v1_0_manutencao_deve_retornar_200()
        {
            var resp = await _client.GetAsync("/api/v1.0/Manutencao?pageNumber=1&pageSize=10");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task POST_api_v1_0_manutencao_deve_retornar_400_quando_moto_nao_existe()
        {
            var body = new Manutencao
            {
                IdMoto = 999,
                DsManutencao = "moto inexistente"
            };

            var resp = await _client.PostAsJsonAsync("/api/v1.0/Manutencao", body);
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
