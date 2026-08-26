namespace B2BCommerceDemo.Core.Interfaces.Events
{
    public interface IEventHandler<TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
}

