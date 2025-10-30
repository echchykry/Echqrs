namespace Echqrs.Tests
{
    public class FirstCommand : ICommand { }
    public class SecondCommand : ICommand { }
    public class MultiCommandHandler :
        ICommandHandler<FirstCommand>,
        ICommandHandler<SecondCommand>
    {
        public static int FirstHitCount = 0;
        public static int SecondHitCount = 0;

        public Task HandleAsync(FirstCommand command)
        {
            FirstHitCount++;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SecondCommand command)
        {
            SecondHitCount++;
            return Task.CompletedTask;
        }
    }

}
