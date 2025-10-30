namespace Echqrs;

    public interface ICommand
    {
    }

    public interface ICommand<TResult> : ICommand
    {
    }
