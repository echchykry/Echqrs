# EchQRS

**EchQRS** is a lightweight and flexible CQRS (Command Query Responsibility Segregation) library for .NET.  
It helps you separate **commands** (write logic) and **queries** (read logic) in a clean, testable, and dependency-injection–friendly way.

---

## 🚀 Features
- Simple and minimal CQRS abstraction.
- Automatic handler registration via reflection.
- Works with commands with or without return types.
- Clean integration with .NET dependency injection.
- Thread-safe execution with scoped lifetimes.

---

## ⚙️ Installation

```bash
dotnet add package EchQRS
```
In your Program.cs:
```csharp

services.AddEchqrs();
```
🧩 Core Interfaces
Commands
Represent write operations that change state.

```csharp

public interface ICommand { }
public interface ICommand<TResult> : ICommand { }
```
Example:

```csharp

public class CreateUserCommand : ICommand<Guid>
{
    public string Name { get; set; } = string.Empty;
}
```
Command Handlers
Contain business logic for executing commands.

```csharp

public interface ICommandHandler<in TCommand>
{
    Task HandleAsync(TCommand command);
}

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command);
}
```
Example:

```csharp

public class CreateUserHandler : ICommandHandler<CreateUserCommand, Guid>
{
    public Task<Guid> HandleAsync(CreateUserCommand command)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"User {command.Name} created with ID {id}");
        return Task.FromResult(id);
    }
}
```
Queries
Represent read-only operations that fetch data.

```csharp

public interface IQuery { }
public interface IQuery<TResult> : IQuery { }
```
Example:

```csharp

public class GetUserByIdQuery : IQuery<UserDto>
{
    public Guid Id { get; set; }
}
```
Query Handlers
Handle queries and return data.

```csharp

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}
```
Example:

```csharp

public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public Task<UserDto> HandleAsync(GetUserByIdQuery query)
    {
        return Task.FromResult(new UserDto(query.Id, "Houssam"));
    }
}
```
Executers
CommandExecuter

```csharp

public interface ICommandExecuter
{
    Task ExecuteAsync<TCommand>(TCommand command);
    Task<TResult> ExecuteWithResultAsync<TResult>(ICommand<TResult> command);
}
```
QueryExecuter

```csharp

public interface IQueryExecuter
{
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
}
```
Usage:

```csharp

await _commandExecuter.ExecuteAsync(new DeleteUserCommand { Id = id });
var user = await _queryExecuter.QueryAsync(new GetUserByIdQuery { Id = id });
```
🧠 How It Works
When you call services.AddEchqrs(), the library:

Scans all assemblies for implementations of ICommandHandler<>, ICommandHandler<,>, and IQueryHandler<,>.

Registers them in the service collection.

Registers CommandExecuter and QueryExecuter as singletons.

Uses IServiceScopeFactory to resolve handlers safely per execution.

🧪 Example Test
```csharp

[Fact]
public async Task ExecuteAsync_Should_Invoke_Handler()
{
    var services = new ServiceCollection()
        .AddEchqrs()
        .AddScoped<ICommandHandler<CreateUserCommand, Guid>, CreateUserHandler>();

    var provider = services.BuildServiceProvider();
    var executer = provider.GetRequiredService<ICommandExecuter>();

    var result = await executer.ExecuteWithResultAsync(new CreateUserCommand { Name = "John" });

    Assert.NotEqual(Guid.Empty, result);
}
```