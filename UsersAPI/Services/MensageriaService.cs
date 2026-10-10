using Contracts.Events;
using Core.Entidade;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UsersAPI.Interface;

namespace UsersAPI.Services
{
    public class MensageriaService : IMensageriaService
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queueName;

        public MensageriaService(IConfiguration configuration)
        {
            _configuration = configuration;
            _hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";
            _userName = _configuration["RabbitMQ:UserName"] ?? "admin";
            _password = _configuration["RabbitMQ:Password"] ?? "admin123";
            _queueName = _configuration["RabbitMQ:QueueName"] ?? "notifications-user-created";
        }

        public async Task EnviarMensagemFila(Usuario usuario)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Declara a fila (criar se não existir)
            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Prepara a mensagem
            UserCreatedEvent menssage = new UserCreatedEvent(Guid.NewGuid(), usuario.Nome, usuario.Email.Endereco, DateTime.Now);
            var messageBody = JsonSerializer.SerializeToUtf8Bytes(menssage);


            // Envia a mensagem
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                body: messageBody);
        }
    }
}
