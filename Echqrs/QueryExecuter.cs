using Microsoft.Extensions.DependencyInjection;

namespace Echqrs;

    internal sealed class QueryExecuter : IQueryExecuter
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryExecuter(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        /// <summary>
        /// Executes a query and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
        /// <param name="query">The query instance.</param>
        /// <returns>The result produced by the registered query handler.</returns>
        /// <exception cref="ArgumentNullException">Thrown when query is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no handler is registered or when HandleAsync cannot be executed.
        /// </exception>
        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            using var scope = _serviceProvider.CreateScope();

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"No query handler found for '{query.GetType().FullName}' producing '{typeof(TResult).FullName}'");

            var method = handlerType.GetMethod("HandleAsync");

            if (method == null)
                throw new InvalidOperationException(
                    $"Handler '{handlerType.FullName}' does not contain a HandleAsync method");

            var task = method.Invoke(handler, new object[] { query }) as Task<TResult>;
            if (task == null)
                throw new InvalidOperationException(
                    $"Query handler invocation failed for '{query.GetType().FullName}'");

            return await task;
        }
    }

