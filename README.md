
# NextParkAPI

API RESTful desenvolvida para o sistema de **Gestão de Motos no Pátio da Mottu**, como parte do projeto da disciplina **Advanced Business Development with .NET**.

---

## 🚀 Funcionalidades

- Cadastro, listagem e remoção de motos
- Vinculação de motos a vagas existentes
- Cadastro e listagem de vagas
- Swagger UI para documentação e testes

---

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core 8 (Web API)
- Entity Framework Core + Oracle
- Oracle.ManagedDataAccess
- Swagger (Swashbuckle)
- Visual Studio / VS Code
- .NET CLI

---

## 📂 Estrutura do Projeto

```
NextParkAPI/
├── Controllers/
│   ├── MotoController.cs
│   └── VagaController.cs
├── Models/
│   ├── Moto.cs
│   └── Vaga.cs
├── Data/
│   └── NextParkContext.cs
├── Program.cs
├── appsettings.json
```

---

## 🎯 Endpoints Disponíveis

### 🔧 Motos

| Método | Rota           | Descrição                     |
|--------|----------------|-------------------------------|
| GET    | /api/Moto      | Lista todas as motos          |
| GET    | /api/Moto/{id} | Busca moto pelo ID            |
| POST   | /api/Moto      | Cadastra uma nova moto        |
| PUT    | /api/Moto/{id} | Atualiza dados da moto        |
| DELETE | /api/Moto/{id} | Remove uma moto do sistema    |

### 🔧 Vagas

| Método | Rota           | Descrição                     |
|--------|----------------|-------------------------------|
| GET    | /api/Vaga      | Lista todas as vagas          |
| GET    | /api/Vaga/{id} | Busca vaga pelo ID            |
| POST   | /api/Vaga      | Cadastra uma nova vaga        |

---

## ▶️ Como Executar Localmente

1. Clone o repositório
2. No arquivo `appsettings.json`, configure a sua string de conexão Oracle:

```json
"ConnectionStrings": {
  "OracleDb": "User Id=seu_usuario;Password=sua_senha;Data Source=localhost:1521/XE;"
}
```

3. Aplique as migrações e atualize o banco:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Rode o projeto:

```bash
dotnet run
```

5. Acesse o Swagger:
```
https://localhost:{porta}/swagger
```

---

## 💡 Observações

- O cadastro de motos depende da existência de vagas válidas.
- O projeto segue o padrão RESTful com boas práticas.
- Ideal para simulação de controle de pátios com múltiplas filiais.

---

## 👨‍💻 Integrantes

- Raphaela Oliveira Tatto – RM: *554983*
- Tiago Ribeiro Capela – RM: *558021*

	
---
