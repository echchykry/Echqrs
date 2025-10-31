namespace Echqrs.Tests
{
    public class CreateUserCommand : ICommand
    {
        public string? Name { get; set; }
    }

    public class CreateUserHandler : ICommandHandler<CreateUserCommand>
    {
        public Task HandleAsync(CreateUserCommand command)
        {
            Assert.False(string.IsNullOrEmpty(command.Name));
            return Task.CompletedTask;
        }
    }

}
