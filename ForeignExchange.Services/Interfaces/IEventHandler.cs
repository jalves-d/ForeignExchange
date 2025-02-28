namespace ForeignExchange.Application.Interfaces
{
    public interface IEventHandler<T>
    {
        Task HandleAsync(T @event);
    }

}
