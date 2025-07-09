namespace Core.Base
{
    public interface IHandlerBase<TResult, TRequest> 
        where TRequest : IRequestBase
        where TResult : IResponseBase
    {
        Task<TResult> Handle(TRequest query);
    }
}
