
// File: src/TransactionService/Services/RabbitMqService.cs
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using AjuaBank.Shared.Interfaces;

namespace AjuaBank.TransactionService.Services;

public class RabbitMqService : IMessageBroker
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost",
            UserName = config["RabbitMQ:User"] ?? "guest",
            Password = config["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare exchanges
        _channel.ExchangeDeclare("transactions", ExchangeType.Topic, durable: true);
    }

    public Task PublishAsync<T>(string exchange, string routingKey, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(string queue, Func<T, Task> handler)
    {
        throw new NotImplementedException("Subscribe handled in consumer services");
    }
}