using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echqrs.Tests
{
    public class EchQrsTests
    {
        private readonly ServiceProvider _provider;
        private readonly ICommandExecuter _commandExecuter;
        private readonly IQueryExecuter _queryExecuter;

        public EchQrsTests()
        {
            var services = new ServiceCollection();
            services.AddEchqrs();
            _provider = services.BuildServiceProvider();

            _commandExecuter = _provider.GetRequiredService<ICommandExecuter>();
            _queryExecuter = _provider.GetRequiredService<IQueryExecuter>();
        }

        [Fact]
        public async Task Execute_Command_Without_Result_Success()
        {
            var cmd = new CreateUserCommand { Name = "Houssam" };

            var exception = await Record.ExceptionAsync(() => _commandExecuter.ExecuteAsync(cmd));

            Assert.Null(exception);
        }

        [Fact]
        public async Task Execute_Command_With_Result_Success()
        {
            var cmd = new CreateUserWithResultCommand { Name = "Test" };

            var result = await _commandExecuter.ExecuteWithResultAsync(cmd);

            Assert.Equal(100, result);
        }

        [Fact]
        public async Task Query_Success()
        {
            var query = new GetUserNameQuery { Id = 1 };

            var result = await _queryExecuter.QueryAsync(query);

            Assert.Equal("Houssam", result);
        }

        [Fact]
        public async Task Execute_Command_No_Handler_Should_Throw()
        {
            var cmd = new DummyCommand();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _commandExecuter.ExecuteAsync(cmd)
            );
        }

        [Fact]
        public async Task Execute_CommandWithResult_No_Handler_Should_Throw()
        {
            var cmd = new DummyCommandWithResult();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _commandExecuter.ExecuteWithResultAsync(cmd)
            );
        }

        [Fact]
        public async Task Query_No_Handler_Should_Throw()
        {
            var query = new DummyQuery();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _queryExecuter.QueryAsync(query)
            );
        }

        [Fact]
        public async Task Query_Should_Return_Correct_Type()
        {
            var query = new GetUserNameQuery { Id = 10 };

            var result = await _queryExecuter.QueryAsync(query);

            Assert.IsType<string>(result);
        }

        private static int _executionCount = 0;

        [Fact]
        public async Task Execute_Should_Invoke_Handler_Each_Time()
        {
            var cmd = new CounntingCommand { Name = "A" };

            await _commandExecuter.ExecuteAsync(cmd);
            await _commandExecuter.ExecuteAsync(cmd);

            Assert.Equal(2, _executionCount);
        }

        [Fact]
        public async Task One_Handler_Two_Commands_Should_Invoke_Correct_Method()
        {
            MultiCommandHandler.FirstHitCount = 0;
            MultiCommandHandler.SecondHitCount = 0;


            await _commandExecuter.ExecuteAsync(new FirstCommand());
            await _commandExecuter.ExecuteAsync(new SecondCommand());
            await _commandExecuter.ExecuteAsync(new FirstCommand());

            // Assert
            Assert.Equal(2, MultiCommandHandler.FirstHitCount);
            Assert.Equal(1, MultiCommandHandler.SecondHitCount);  
        }
        [Fact]
        public async Task Should_resolve_main_handler_by_given_interface()
        {
            
            var requests = new ICommand<int>[] { new CreateUserWithResultCommand { Name = "Test" } };
            var result = await _commandExecuter.ExecuteWithResultAsync(requests[0]);
            Assert.Equal(100, result);


        }

        public class CountingHandler : ICommandHandler<CounntingCommand>
        {
            public Task HandleAsync(CounntingCommand command)
            {
                _executionCount++;
                return Task.CompletedTask;
            }
        }
    }

    public class DummyCommand : ICommand { }

    public class DummyCommandWithResult : ICommand<int> { }

    public class DummyQuery : IQuery<string> { }



}
