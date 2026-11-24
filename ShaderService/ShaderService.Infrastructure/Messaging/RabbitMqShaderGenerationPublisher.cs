
using System.Text.Json;
using Contracts.ShaderGeneration;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using ShaderService.Application.Abstractions;

namespace ShaderService.Infrastructure.Messaging;

public sealed class RabbitMqShaderGenerationPublisher
    : IShaderGenerationPublisher, IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory;

    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _routingKey;

    private readonly SemaphoreSlim _publishLock = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqShaderGenerationPublisher(
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMQ");

        _exchangeName = settings["ExchangeName"] ?? "shader.exchange";
        _queueName = settings["QueueName"] ?? "shader.generate";
        _routingKey = settings["RoutingKey"] ?? "shader.generate";

        _connectionFactory = new ConnectionFactory
        {
            HostName = settings["HostName"] ?? "localhost",
            Port = int.TryParse(settings["Port"], out var port)
                ? port
                : 5672,
            UserName = settings["UserName"] ?? "guest",
            Password = settings["Password"] ?? "guest",

            AutomaticRecoveryEnabled = true,
            ClientProvidedName = "ShaderService.Api"
        };
    }

    public async Task PublishAsync(
        Guid generationId,
        Guid userId,
        string prompt,
        CancellationToken cancellationToken)
    {
        await _publishLock.WaitAsync(cancellationToken);

        try
        {
            await EnsureChannelAsync(cancellationToken);

            var command = new GenerateShaderCommand(
                generationId,
                userId,
                prompt);

            var body = JsonSerializer.SerializeToUtf8Bytes(command);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = generationId.ToString()
            };

            await _channel!.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: _routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _publishLock.Release();
        }
    }

    private async Task EnsureChannelAsync(
        CancellationToken cancellationToken)
    {
        if (_connection is null || !_connection.IsOpen)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            _connection = await _connectionFactory
                .CreateConnectionAsync(cancellationToken);
        }

        if (_channel is null || !_channel.IsOpen)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            var channelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true);

            _channel = await _connection!.CreateChannelAsync(
                channelOptions,
                cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: _routingKey,
                cancellationToken: cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();

        _publishLock.Dispose();
    }
}
