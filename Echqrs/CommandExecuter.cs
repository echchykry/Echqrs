using Microsoft.Extensions.DependencyInjection;

namespace Echqrs;

internal sealed class CommandExecuter : ICommandExecuter
{
    private readonly IServiceProvider _provider;

    public CommandExecuter(IServiceProvider serviceProvider)
        => _provider = serviceProvider;

    /// <summary>
    /// Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <param name="command">The command instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered.</exception>
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        using var scope = _provider.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        if (handler == null)
            throw new InvalidOperationException($"No handler found for command '{typeof(TCommand).FullName}'");


        await handler.HandleAsync(command);
    }

    /// <summary>
    /// Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of response.</typeparam>
    /// <param name="command">The command instance.</param>
    /// <returns>The result returned by the handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered or invocation fails.</exception>
    public async Task<TResult> ExecuteWithResultAsync<TResult>(ICommand<TResult> command)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var scope = _provider.CreateScope();

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"No handler found for command '{command.GetType().FullName}'");

        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
            throw new InvalidOperationException(
                $"Handler '{handlerType.FullName}' does not contain HandleAsync method");

        var task = method.Invoke(handler, new object[] { command }) as Task<TResult>;

        if (task == null)
            throw new InvalidOperationException(
                $"Handler invocation failed for command '{command.GetType().FullName}'");

        return await task;
    }
}
