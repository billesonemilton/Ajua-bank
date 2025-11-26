using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using AjuaBank.Shared.Events;
using AjuaBank.Shared.DTOs;

namespace AjuaBank.FraudDetectionService.Services;

public class TransactionConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IModel? _channel;

    public TransactionConsumer(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);

        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost",
            UserName = _config["RabbitMQ:User"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("transactions", ExchangeType.Topic, durable: true);
        _channel.QueueDeclare("fraud-detection-queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("fraud-detection-queue", "transactions", "transaction.created");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var evt = JsonSerializer.Deserialize<TransactionCreatedEvent>(message);

            if (evt != null)
            {
                await ProcessTransactionEventAsync(evt);
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume("fraud-detection-queue", false, consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessTransactionEventAsync(TransactionCreatedEvent evt)
    {
        using var scope = _serviceProvider.CreateScope();
        var detector = scope.ServiceProvider.GetRequiredService<FraudDetector>();

        var request = new FraudCheckRequest(
            evt.TransactionId,
            evt.UserId,
            evt.Amount,
            evt.Type,
            evt.Timestamp,
            evt.ToAccountNumber
        );

        var result = await detector.AnalyzeTransactionAsync(request);

        Console.WriteLine($"Fraud check for {evt.TransactionId}: {result.RiskLevel} ({result.RiskScore})");
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
