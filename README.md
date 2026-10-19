# UsersAPI

Microsserviço REST para cadastro e gerenciamento de usuários, níveis de acesso e autenticação da aplicação TechChallenge. A API persiste os usuários em SQL Server, armazena senhas com hash BCrypt, emite tokens JWT e publica um evento de usuário criado no RabbitMQ.

## Tecnologias

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core e SQL Server
- JWT Bearer para autenticação e autorização por papel
- RabbitMQ para eventos de cadastro
- Swagger para explorar e testar a API

## Estrutura da solução

- `UsersAPI`: endpoints HTTP, serviços, middlewares, configuração e autenticação.
- `Core`: entidades, DTOs, contratos de repositório e validações.
- `Infra`: persistência com Entity Framework Core, repositórios e migrations.

## Funcionalidades e endpoints

A rota base dos controladores é o nome do controlador. Em ambiente `Development`, a documentação interativa fica em `/swagger`.

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `POST` | `/Usuario` | Público | Cadastra usuário comum e publica o evento `UserCreatedEvent`. |
| `GET` | `/Autenticacao?email={email}&senha={senha}` | Público | Autentica e retorna um JWT válido por 30 minutos. |
| `GET` | `/Usuario/me` | JWT | Consulta os dados do usuário autenticado. |
| `PUT` | `/Usuario/{id}` | JWT | Atualiza os dados; usuário comum só pode atualizar a própria conta. |
| `GET` | `/Usuario` | Papel `Admin` | Lista usuários; aceita filtros `nome`, `email` e `nivelAcesso`. |
| `GET` | `/Usuario/{id}` | Papel `Admin` | Consulta usuário por identificador. |
| `POST` | `/Usuario/admin` | Papel `Admin` | Cadastra outro administrador. |
| `DELETE` | `/Usuario/{id}` | Papel `Admin` | Desativa o usuário. |

Exemplo do corpo para `POST /Usuario` (também usado em `POST /Usuario/admin`):

```json
{
  "nome": "Maria Silva",
  "email": "maria@example.com",
  "senha": "SenhaForte123!"
}
```

Para acessar rotas protegidas, use o token retornado pela autenticação no cabeçalho `Authorization: Bearer {token}`. As migrations são aplicadas automaticamente quando a API inicia.

As migrations provisionam uma conta administradora inicial:

| Campo | Valor |
|---|---|
| Nome | `admin` |
| E-mail | `admin@gmail.com` |
| Senha | `Fiap2026@` |

Use essa conta para autenticar e obter um token JWT antes de testar as rotas restritas a `Admin`. A senha é armazenada no banco como hash BCrypt. Altere as credenciais padrão antes de qualquer uso fora do ambiente de avaliação.

## Configuração

O ASP.NET Core permite substituir as configurações do `appsettings.json` por variáveis de ambiente. Para nomes hierárquicos, use `__` (dois sublinhados) no lugar de `:`.

| Variável | Uso | Valor padrão / observação |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | String de conexão SQL Server. | Localmente, `appsettings.json` usa `localhost`, banco `UserApi` e autenticação integrada do Windows. No Compose, aponta para o serviço `db`. |
| `Jwt__Key` | Chave simétrica usada para assinar e validar os JWTs. | Obrigatória no Compose; use uma chave secreta com pelo menos 32 caracteres e não reutilize a chave de exemplo em produção. |
| `Jwt__Issuer` | Emissor dos tokens. | `TechChallenge`; o Compose também define esse valor. |
| `RabbitMq__Host` | Host RabbitMQ definido para a API pelo Compose. | `rabbitmq` (nome do serviço na rede Docker). |
| `RABBITMQ_USER` | Usuário RabbitMQ criado no container e enviado à API pelo Compose. | Obrigatória no Compose; escolha um usuário diferente de `guest` para conexões entre contêineres. |
| `RABBITMQ_PASSWORD` | Senha do usuário criado no container e enviada à API pelo Compose. | Obrigatória no Compose; use uma senha forte. |
| `RabbitMQ__Host` | Host para execução local fora do Compose. | `localhost`. |
| `RabbitMQ__Username` | Usuário RabbitMQ para execução local. | `guest`. |
| `RabbitMQ__Password` | Senha RabbitMQ para execução local. | `guest`. |
| `RabbitMQ__QueueName` | Fila para eventos de criação de usuário. | `notifications-user-created`. |
| `DB_PASSWORD` | Senha `sa` do SQL Server no Docker Compose. | Obrigatória para executar `docker compose`; defina uma senha que atenda à política do SQL Server. |

O serviço de mensageria lê as configurações `RabbitMQ:Host`, `RabbitMQ:Username`, `RabbitMQ:Password` e `RabbitMQ:QueueName`.

## Executar localmente

Pré-requisitos: .NET SDK 8, SQL Server acessível pela string de conexão e RabbitMQ acessível pelo serviço. No Windows, a configuração padrão do SQL Server usa autenticação integrada. O RabbitMQ local pode ser iniciado com:

```powershell
docker run -d --name users-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:management
```

Com as credenciais padrão `guest`/`guest`, configure uma string de conexão para o seu SQL Server e inicie a API:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=UserApi;Trusted_Connection=True;TrustServerCertificate=True;"
$env:Jwt__Key = "chave-local-de-desenvolvimento-com-pelo-menos-32-caracteres"
dotnet run --project UsersAPI/UsersAPI.csproj
```

Abra `http://localhost:5226/swagger` (perfil HTTP) ou `https://localhost:7209/swagger` (perfil HTTPS). O perfil de desenvolvimento abre o Swagger e a API aplica as migrations pendentes durante a inicialização.

## Executar com Docker Compose

Na raiz do repositório, crie um arquivo `.env` com os valores exigidos pelo `docker-compose.yml`:

```dotenv
DB_PASSWORD=UmaSenhaForte_SQL_2026!
JWT_KEY=chave-jwt-local-com-pelo-menos-32-caracteres
RABBITMQ_USER=usersapi
RABBITMQ_PASSWORD=UmaSenhaForte_Rabbit_2026!
```

Suba os contêineres:

```powershell
docker compose up --build
```

A API ficará disponível em `http://localhost:5000/swagger`; o SQL Server ficará exposto na porta `1434` do host. O RabbitMQ ficará disponível na porta `5672`, e a interface de gerenciamento em `http://localhost:15672`, usando o usuário e a senha definidos em `RABBITMQ_USER` e `RABBITMQ_PASSWORD`. O Compose aguarda o SQL Server e o RabbitMQ passarem pelos respectivos health checks antes de iniciar a API. A aplicação cria/aplica migrations no banco `UserApi`.

> O Compose inicia os três contêineres: SQL Server, API e RabbitMQ. `RABBITMQ_USER` e `RABBITMQ_PASSWORD` configuram tanto o usuário/senha inicial do broker (`RABBITMQ_DEFAULT_USER`/`RABBITMQ_DEFAULT_PASS`) quanto as credenciais utilizadas pela API, mantendo os valores alinhados. A API conecta-se ao broker pelo host `rabbitmq`. Use um usuário diferente de `guest`, pois o RabbitMQ restringe `guest` a conexões locais ao próprio contêiner. A API depende de os health checks do SQL Server e do RabbitMQ indicarem que estão saudáveis antes de iniciar.
