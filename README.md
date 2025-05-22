
## 🚀 Descrição do Projeto

Nosso projeto tem como objetivo otimizar a organização do pátio da Mottu. Para isso, desenvolvemos um aplicativo mobile voltado para os operadores de pátio e demais funcionários, permitindo localizar rapidamente qualquer moto dentro do espaço.

A solução utiliza câmeras instaladas em pontos estratégicos do pátio, combinadas com um sistema de mapeamento de vagas padronizadas (ex: "A1", "A2", etc). O usuário poderá inserir a placa da moto no aplicativo, e o sistema informará em qual vaga ela está estacionada.

Além disso, ao receber uma nova moto, o operador poderá cadastrá-la no sistema e, automaticamente, o aplicativo irá sugerir uma vaga livre, otimizando o processo de alocação e evitando desorganização.


---

## 🚀 Funcionalidades

- Cadastro, listagem e remoção de motos
- Vinculação de motos a vagas existentes
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
│   └── MotoController.cs
├── Models/
│   └── Moto.cs
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

- O cadastro de motos depende da existência de vagas válidas no banco.
- O projeto segue o padrão RESTful com boas práticas.
- Ideal para simulação de controle de pátios com múltiplas filiais.

---

## 👨‍💻 Integrantes

- Raphaela Oliveira Tatto – RM: *554983*
- Tiago Ribeiro Capela – RM: *558021*

	
---
