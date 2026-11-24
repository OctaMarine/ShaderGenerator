using System.Text.Json;
using Contracts.ShaderGeneration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShaderService.Application.ShaderGenerations;

namespace ShaderService.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var settings = _configuration.GetSection("RabbitMQ");

        var factory = new ConnectionFactory
        {
            HostName = settings["HostName"] ?? "localhost",
            Port = int.TryParse(settings["Port"], out var port)
                ? port
                : 5672,
            UserName = settings["UserName"] ?? "guest",
            Password = settings["Password"] ?? "guest",
            AutomaticRecoveryEnabled = true,
            ClientProvidedName = "ShaderService.Worker"
        };

        await using var connection =
            await factory.CreateConnectionAsync(stoppingToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        var exchangeName = settings["ExchangeName"]!;
        var queueName = settings["QueueName"]!;
        var routingKey = settings["RoutingKey"]!;
        
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey,
            cancellationToken: stoppingToken);
        
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();

            GenerateShaderCommand? command;

            try
            {
                command = JsonSerializer
                    .Deserialize<GenerateShaderCommand>(body);
            }
            catch (JsonException exception)
            {
                _logger.LogError(
                    exception,
                    "Invalid message received.");
                
                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                return;
            }

            if (command is null)
            {
                _logger.LogError("Received empty command.");

                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                return;
            }

            try
            {
                _logger.LogInformation(
                    "Received shader generation: {GenerationId}",
                    command.GenerationId);

                await using var scope = _scopeFactory.CreateAsyncScope();

                var processor = scope.ServiceProvider
                    .GetRequiredService<ProcessShaderGenerationService>();

                await processor.ProcessAsync(
                    command.GenerationId,
                    stoppingToken);
                
                await channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "Shader generation completed: {GenerationId}",
                    command.GenerationId);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                // При остановке Worker не подтверждаем сообщение.
                // После закрытия соединения RabbitMQ сможет доставить его повторно.
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to process generation: {GenerationId}",
                    command.GenerationId);
                
                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        var consumerTag = await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "Shader Worker started. Consumer tag: {ConsumerTag}",
            consumerTag);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Shader Worker is stopping.");
        }
    }
}