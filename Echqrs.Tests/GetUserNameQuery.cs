namespace Echqrs.Tests
{
    public class GetUserNameQuery : IQuery<string>
    {
        public int Id { get; set; }
    }
    public class GetUserNameHandler : IQueryHandler<GetUserNameQuery, string>
    {
        public Task<string> HandleAsync(GetUserNameQuery query)
        {
            return Task.FromResult("Houssam");
        }
    }

}
