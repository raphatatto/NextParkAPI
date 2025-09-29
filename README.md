# NextParkAPI

## 👥 Integrantes
- Raphaela Oliveira Tatto – RM 554983
- Tiago Ribeiro Capela – RM 558021

---

## 🧭 Justificativa da Arquitetura
- **ASP.NET Core Web API**: fornece o backbone RESTful necessário para exposição de recursos HTTP, com middleware de roteamento, validação de modelo e Swagger já integrados para documentação.
- **Entity Framework Core com provider Oracle**: mapeia diretamente as tabelas existentes (`MOTO`, `VAGA`, `MANUTENCAO`), permitindo reutilizar o esquema legado sem scripts adicionais e aproveitando o controle de rastreamento do EF para as operações CRUD.
- **Camadas enxutas**: controllers concentram as regras de negócio do domínio do pátio, delegando o acesso a dados ao `NextParkContext`. Essa abordagem reduz complexidade e atende ao escopo acadêmico do projeto.
- **Padrões REST**: endpoints seguem convenções de recursos (`/api/Moto`, `/api/Vaga`, `/api/Manutencao`), retornam códigos HTTP adequados e entregam envelopes de resposta consistentes (`PagedResponse`, `ResourceResponse`) para facilitar consumo por clientes front-end.

---

## ▶️ Instruções de Execução da API
1. **Pré-requisitos**: .NET 8 SDK instalado e acesso a uma instância Oracle acessível pelo aplicativo.
2. **Clonar o repositório**:
   ```bash
   git clone <url-do-repositorio>
   cd NextParkAPI
   ```
3. **Configurar a string de conexão** em `appsettings.json` e `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "OracleDb": "User Id=seu_usuario;Password=sua_senha;Data Source=localhost:1521/XEPDB1;"
   }
   ```
4. **Aplicar migrações (opcional)**, caso as tabelas ainda não existam na base Oracle:
   ```bash
   dotnet ef database update
   ```
5. **Executar a API**:
   ```bash
   dotnet run
   ```
6. **Acessar o Swagger UI** em `http://localhost/swagger` (ou a porta configurada).

---

## 📚 Exemplos de Uso dos Endpoints
### 🏍️ Motos (`/api/Moto`)
- **Listar**:
  ```bash
  curl "http://localhost:5000/api/Moto?pageNumber=1&pageSize=10"
  ```
- **Criar**:
  ```bash
  curl -X POST "http://localhost:5000/api/Moto" \
    -H "Content-Type: application/json" \
    -d '{
          "idMoto": 1,
          "modelo": "Honda CG",
          "placa": "ABC1234",
          "ano": 2023
        }'
  ```

### 🅿️ Vagas (`/api/Vaga`)
- **Listar**:
  ```bash
  curl "http://localhost:5000/api/Vaga?pageNumber=1&pageSize=10"
  ```
- **Criar**:
  ```bash
  curl -X POST "http://localhost:5000/api/Vaga" \
    -H "Content-Type: application/json" \
    -d '{
          "idVaga": 101,
          "areaVaga": "A1",
          "stVaga": "L",
          "idPatio": 1
        }'
  ```

### 🔧 Manutenções (`/api/Manutencao`)
- **Listar**:
  ```bash
  curl "http://localhost:5000/api/Manutencao?pageNumber=1&pageSize=10"
  ```
- **Criar**:
  ```bash
  curl -X POST "http://localhost:5000/api/Manutencao" \
    -H "Content-Type: application/json" \
    -d '{
          "idManutencao": 10,
          "dsManutencao": "Troca de óleo",
          "dtInicio": "2024-05-01",
          "dtFim": "2024-05-02",
          "idMoto": 1
        }'
  ```

