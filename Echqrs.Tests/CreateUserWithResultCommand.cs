namespace Echqrs.Tests
{
    public class CreateUserWithResultCommand : ICommand<int>
    {
        public string Name { get; set; }
    }

    public class CreateUserWithResultHandler : ICommandHandler<CreateUserWithResultCommand, int>
    {
        public Task<int> HandleAsync(CreateUserWithResultCommand command)
        {
            return Task.FromResult(100); // fake created ID
        }
    }
}
