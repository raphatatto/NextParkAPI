## 🚀 Visão Geral

API RESTful construída em ASP.NET Core para apoiar a gestão dos pátios da Mottu. O serviço expõe recursos para cadastro e consulta
de motos, vagas e ordens de manutenção, permitindo que operadores acompanhem o ciclo completo de utilização do pátio.

---

## 🧭 Justificativa de Arquitetura

- **Domínio**: as entidades `Moto`, `Vaga` e `Manutenção` foram escolhidas com base no diagrama de banco fornecido pelo professor,
  cobrindo o fluxo principal de alocação de veículos no pátio e o histórico de intervenções. Essas três tabelas já existem no
  Oracle e foram mapeadas diretamente com Entity Framework Core.
- **Camada de dados**: o `NextParkContext` utiliza `DbContext` do EF Core com provider Oracle, respeitando a modelagem física
  (nomes de tabelas e colunas) para evitar divergências entre o código e o banco legado.
- **Boas práticas REST**: todos os endpoints implementam paginação por query string (`pageNumber`, `pageSize`), retornam HATEOAS
  (links de navegação) e utilizam códigos HTTP adequados (200, 201, 204, 400, 404). As respostas seguem um envelope padronizado
  (`PagedResponse` e `ResourceResponse`) para facilitar a integração com clientes front-end ou mobile.
- **Documentação**: o Swagger/OpenAPI é configurado por padrão no projeto para permitir exploração e testes manuais.

---

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core 8 (Web API)
- Entity Framework Core 8 com Oracle.ManagedDataAccess
- Swagger (Swashbuckle)
- .NET CLI

---

## 📂 Estrutura do Projeto

```
NextParkAPI/
├── Controllers/
│   ├── ManutencaoController.cs
│   ├── MotoController.cs
│   └── VagaController.cs
├── Data/
│   └── NextParkContext.cs
├── Models/
│   ├── Manutencao.cs
│   ├── Moto.cs
│   ├── Vaga.cs
│   └── Responses/
│       ├── Link.cs
│       ├── PagedResponse.cs
│       └── ResourceResponse.cs
├── Program.cs
└── README.md
```

---

## ▶️ Como Executar Localmente

1. Clone o repositório público.
2. Atualize a string de conexão Oracle nos arquivos `appsettings.json` e `appsettings.Development.json`:

   ```json
   "ConnectionStrings": {
     "OracleDb": "User Id=seu_usuario;Password=sua_senha;Data Source=localhost:1521/XE;"
   }
   ```

3. (Opcional) Gere as migrações e atualize o banco, caso ainda não existam as tabelas:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. Execute a API:

   ```bash
   dotnet run
   ```

5. Acesse a documentação interativa no Swagger UI: `http://localhost:80/swagger`.

---

## 🎯 Endpoints Principais

### 🔧 Motos (`/api/Moto`)

| Método | Descrição |
|--------|-----------|
| GET    | Lista motos com paginação (`pageNumber`, `pageSize`). |
| GET /{id} | Retorna os detalhes de uma moto específica. |
| POST   | Cria uma nova moto. |
| PUT /{id} | Atualiza uma moto existente. |
| DELETE /{id} | Remove uma moto. |

**Exemplo de requisição:**

```bash
curl "http://localhost:80/api/Moto?pageNumber=1&pageSize=5"
```

### 🅿️ Vagas (`/api/Vaga`)

| Método | Descrição |
|--------|-----------|
| GET    | Lista vagas com paginação. |
| GET /{id} | Retorna uma vaga específica. |
| POST   | Cria uma nova vaga. |
| PUT /{id} | Atualiza uma vaga existente. |
| DELETE /{id} | Remove uma vaga. |

**Exemplo de requisição:**

```bash
curl -X POST "http://localhost:80/api/Vaga" \
  -H "Content-Type: application/json" \
  -d '{"idVaga":101,"areaVaga":"A1","stVaga":"L","idPatio":1}'
```

### 🔧 Manutenções (`/api/Manutencao`)

| Método | Descrição |
|--------|-----------|
| GET    | Lista ordens de manutenção com paginação. |
| GET /{id} | Detalhes de uma manutenção. |
| POST   | Registra uma nova manutenção para uma moto existente. |
| PUT /{id} | Atualiza dados de uma manutenção. |
| DELETE /{id} | Remove uma manutenção. |

**Exemplo de requisição:**

```bash
curl -X POST "http://localhost:80/api/Manutencao" \
  -H "Content-Type: application/json" \
  -d '{"idManutencao":10,"dsManutencao":"Troca de óleo","dtInicio":"2024-05-01","dtFim":"2024-05-02","idMoto":1}'
```

Cada resposta inclui links HATEOAS (`self`, `update`, `delete`, `next`, `previous`) para facilitar a navegação entre recursos e
operações permitidas.

---

## ✅ Testes e Qualidade

- Para validar a compilação e futuras suítes de teste, utilize: `dotnet test`
- Caso não existam projetos de teste configurados, o comando acima servirá como referência oficial para quando forem adicionados.

---

## 👥 Integrantes

- Raphaela Oliveira Tatto – RM: *554983*
- Tiago Ribeiro Capela – RM: *558021*

---
