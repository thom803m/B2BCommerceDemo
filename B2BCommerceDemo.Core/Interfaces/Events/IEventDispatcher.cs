namespace B2BCommerceDemo.Core.Interfaces.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent @event);
    }
}

