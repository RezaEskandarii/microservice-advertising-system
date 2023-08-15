namespace AdvertisingSystem.Contract.Interfaces;

public interface ICommandHandler<TCommand> where TCommand : class
{
    Task HandleAsync(TCommand command);
}