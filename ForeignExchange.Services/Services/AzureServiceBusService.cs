using Azure.Messaging.ServiceBus;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Events;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ForeignExchange.Application.Services
{
    public class AzureServiceBusService : IMessageService
{
    private readonly string _connectionString;
    private readonly string _queueName;
    private readonly List<object> _handlers = new();

    public AzureServiceBusService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureServiceBus:ConnectionString"];
        _queueName = configuration["AzureServiceBus:QueueName"];
    }

    public async Task PublishAsync<T>(T @event)
    {
        await using var client = new ServiceBusClient(_connectionString);
        ServiceBusSender sender = client.CreateSender(_queueName);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(@event));
        await sender.SendMessageAsync(message);
    }

    public async Task StartListeningAsync()
    {
        await using var client = new ServiceBusClient(_connectionString);
        ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async args =>
        {
            var eventType = typeof(ExchangeRateUpdatedEvent);
            var handler = _handlers.OfType<IEventHandler<ExchangeRateUpdatedEvent>>().FirstOrDefault();

            if (handler != null)
            {
                var @event = JsonSerializer.Deserialize<ExchangeRateUpdatedEvent>(args.Message.Body.ToString());
                await handler.HandleAsync(@event);
            }

            await args.CompleteMessageAsync(args.Message);
        };

        processor.ProcessErrorAsync += async args =>
        {
            Console.WriteLine($"Error in Service Bus processing: {args.Exception.Message}");
        };

        await processor.StartProcessingAsync();
    }

    public void Subscribe<T>(IEventHandler<T> handler)
    {
        _handlers.Add(handler);
    }
}
}
