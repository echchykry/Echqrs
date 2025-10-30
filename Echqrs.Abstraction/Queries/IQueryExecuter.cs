namespace Echqrs;

    public interface IQueryExecuter
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
