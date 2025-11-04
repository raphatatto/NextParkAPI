# NextParkAPI

NextParkAPI é uma API REST desenvolvida em ASP.NET Core 8 para apoiar a gestão de pátios de estacionamento de motos. O projeto expõe recursos versionados para cadastro de motos, vagas, manutenções e autenticação de usuários, além de entregar telemetria, health checks e um endpoint de predição de manutenção com ML.NET. O repositório também inclui uma bateria de testes automatizados (unitários e de integração) em xUnit.

## 👤 Integrantes
- Raphaela Oliveira Tatto – RM 554983
- Tiago Ribeiro Capela – RM 558021

## 📁 Estrutura do repositório

| Caminho | Descrição |
| --- | --- |
| `NextParkAPI/` | Projeto principal ASP.NET Core com controllers, modelos, serviços e configurações. |
| `MinhaAPITeste/` | Projeto de testes automatizados (xUnit) com cenários unitários e de integração usando `WebApplicationFactory`. |
| `NextParkAPI.sln` | Solution que referencia os dois projetos. |

## 📺 Arquitetura e principais recursos

- **ASP.NET Core Web API 8** com versionamento por segmento de rota (`/api/v1.0/...`).
- **Entity Framework Core** mapeando o esquema Oracle existente (`TB_NEXTPARK_*`).
- **Autenticação JWT** com serviço de emissão de tokens configurado via `JwtOptions`.
- **Swagger/OpenAPI** com descrições enriquecidas e suporte à autenticação Bearer.
- **Health checks** (endpoint `/health`) e health check customizado `/api/v1.0/Health`.
- **OpenTelemetry + Jaeger exporter** configurados para rastreamento distribuído.
- **Serviço de ML.NET** para predição de necessidade de manutenção (`POST /api/v1.0/Manutencao/predict`).

## 👨‍💻 Pré-requisitos

1. **SDK .NET 8.0** para compilar e executar o projeto.
2. **Banco Oracle acessível** (pode ser Oracle XE local ou instância remota).
3. (Opcional) **Jaeger** rodando para consumir os traces gerados pela API.
4. (Opcional) `dotnet-ef` se desejar aplicar migrações ou administrar o schema via CLI do EF Core.

## 🛠️ Configuração

A configuração padrão reside em `NextParkAPI/appsettings.json`. Recomenda-se criar um `appsettings.Development.json` ou usar variáveis de ambiente para credenciais sensíveis.

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=seu_usuario;Password=sua_senha;Data Source=host:1521/sid;"
  },
  "Jwt": {
    "Issuer": "NextParkAPI",
    "Audience": "NextParkAPI.Clients",
    "Key": "mude-esta-chave-para-uma-bem-grande-e-secreta",
    "ExpiresMinutes": 60
  },
  "Jaeger": {
    "Host": "localhost",
    "Port": "6831"
  }
}
```

### Variáveis de ambiente suportadas

| Variável | Descrição |
| --- | --- |
| `ConnectionStrings__OracleDb` | String de conexão Oracle utilizada pelo `NextParkContext`. |
| `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Key`, `Jwt__ExpiresMinutes` | Configurações do JWT Bearer. |
| `Jaeger__Host`, `Jaeger__Port` | Destino para exportar traces via Jaeger. |

## ▶️ Execução local da API

1. Restaurar dependências:
   ```bash
   dotnet restore NextParkAPI.sln
   ```
2. (Opcional) Aplicar migrações ou criar a base se estiver usando migrations:
   ```bash
   dotnet ef database update --project NextParkAPI/NextParkAPI.csproj
   ```
3. Executar a aplicação:
   ```bash
   dotnet run --project NextParkAPI/NextParkAPI.csproj
   ```
4. Acessar o Swagger UI em `http://localhost:8080/swagger/index.html` e o health check em `http://localhost:8080/health`.

> 👍 Dica: utilize `dotnet watch run --project NextParkAPI/NextParkAPI.csproj` para reload automático em ambiente de desenvolvimento.

## 🛥️ Execução com Docker

1. Construir a imagem (contexto raiz do repositório):
   ```bash
   docker build -t nextpark-api -f NextParkAPI/Dockerfile .
   ```
2. Executar o container expondo a porta 8080:
   ```bash
   docker run --rm -p 8080:8080 \
     -e ConnectionStrings__OracleDb="User Id=usuario;Password=senha;Data Source=host:1521/sid" \
     -e Jwt__Key="chave-super-secreta" \
     nextpark-api
   ```

## 🔍 Endpoints principais

| Recurso | Rota | Operações destacadas |
| --- | --- | --- |
| **Autenticação** | `POST /api/v1.0/Auth/register`, `POST /api/v1.0/Auth/login` | Cadastro e login de usuários com emissão de JWT. |
| **Motos** | `/api/v1.0/Moto` | CRUD com paginação e HATEOAS. |
| **Vagas** | `/api/v1.0/Vaga` | CRUD de vagas de estacionamento. |
| **Manutenções** | `/api/v1.0/Manutencao` | CRUD, paginação, HATEOAS e `POST /predict` para inferência com ML.NET. |
| **Health** | `/api/v1.0/Health`, `/health` | Health check interno (service) e pipeline do ASP.NET Core. |

Todas as rotas são documentadas no Swagger. Endpoints protegidos exigem enviar `Authorization: Bearer {token}` com o JWT obtido no login.

## 🧠 Predição de manutenção

O serviço `ManutencaoModelService` encapsula um modelo ML.NET para prever a necessidade de manutenção com base em dados como quilometragem, idade e temperatura do motor. Utilize o endpoint:

```http
POST /api/v1.0/Manutencao/predict
Content-Type: application/json

{
  "quilometragem": 12000,
  "idadeMotoMeses": 18,
  "temperaturaMotor": 92
}
```

A resposta inclui a probabilidade e o sinalizador `necessitaManutencao`.

## 📝 Testes automatizados

O projeto `MinhaAPITeste` cobre cenários unitários e de integração:

- Os testes de integração utilizam `CustomWebApplicationFactory`, substituem o `NextParkContext` por uma base InMemory e fazem seed de dados para exercitar os endpoints reais (`HttpClient`).
- As dependências de teste incluem `Microsoft.AspNetCore.Mvc.Testing`, `FluentAssertions`, `Moq` e `coverlet.collector` para medições de cobertura.

### Como executar

1. Garantir o SDK .NET 8 instalado e a solution restaurada.
2. Rodar todos os testes:
   ```bash
   dotnet test NextParkAPI.sln
   ```
3. Para gerar cobertura, habilite o coletor do coverlet:
   ```bash
   dotnet test NextParkAPI.sln /p:CollectCoverage=true
   ```

Os testes de integração não dependem de Oracle, pois utilizam `UseInMemoryDatabase`.

## 🔗 Outras ferramentas

- `NextParkAPI.http`: coleção de requisições pronta para REST Client (VS Code) ou JetBrains.
- `Program.cs`: configura autenticação, health checks, versionamento e OpenTelemetry.
- `Utils/TokenService`: gera tokens JWT alinhados com as configurações de `JwtOptions`.

---

> 💡 Para contribuições, abra PRs descrevendo claramente a mudança e lembre-se de manter a cobertura dos testes.
