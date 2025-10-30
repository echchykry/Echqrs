using Microsoft.Extensions.DependencyInjection;

namespace Echqrs
{
    public static class Reflection
    {
        /// <summary>
        /// Registers CQRS command and query handlers found in loaded assemblies.
        /// </summary>
        /// <remarks>
        /// This method scans for ICommandHandler and IQueryHandler implementations.
        /// </remarks>
        /// <param name="services">The IServiceCollection for DI registration.</param>
        /// <returns>The same IServiceCollection for fluent chaining.</returns>
        public static IServiceCollection AddEchqrs(this IServiceCollection services)
        {
            services.RegisterHandlers(typeof(ICommandHandler<>));
            services.RegisterHandlers(typeof(ICommandHandler<,>));
            services.RegisterHandlers(typeof(IQueryHandler<,>));


            services.AddScoped<ICommandExecuter, CommandExecuter>();
            services.AddScoped<IQueryExecuter, QueryExecuter>();

            return services;
        }

        private static bool IsConcrete(this Type type)
                    => !type.IsAbstract && !type.IsInterface;

        private static void RegisterHandlers(this IServiceCollection services, Type handlerGenericType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var handlers = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsConcrete())
                .Select(t => new
                {
                    Impl = t,
                    Interfaces = t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerGenericType)
                })
                .Where(x => x.Interfaces.Any());

            foreach (var handler in handlers)
            {
                foreach (var i in handler.Interfaces)
                {
                    services.AddScoped(i, handler.Impl);
                }
            }
        }

    }
}
