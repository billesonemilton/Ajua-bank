// File: src/SharedLibrary/Interfaces/IMessageBroker.cs
namespace AjuaBank.Shared.Interfaces;

public interface IMessageBroker
{
    Task PublishAsync<T>(string exchange, string routingKey, T message);
    Task SubscribeAsync<T>(string queue, Func<T, Task> handler);
}