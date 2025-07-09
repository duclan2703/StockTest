using Core.Base;

namespace Core
{
    public interface IQueryHandler<TQuery, TResult> : IHandlerBase<TResult, TQuery>
        where TQuery : IQuery<TQuery, TResult>
        where TResult : IResponseBase
    {
    }
}
