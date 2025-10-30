namespace Echqrs;

    public interface ICommandExecuter
    {
        Task ExecuteAsync<TCommand>(TCommand command) where TCommand : class, ICommand;

        Task<TResult> ExecuteWithResultAsync<TResult>(ICommand<TResult> command);

    }
