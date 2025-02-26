namespace ForeignExchange.Application.Interfaces
{
    public interface IMessageService
    {
        Task PublishAsync<T>(T @event);
        Task StartListeningAsync();
        void Subscribe<T>(IEventHandler<T> handler);
    }

}