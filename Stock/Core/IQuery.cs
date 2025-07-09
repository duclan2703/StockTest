using Core.Base;

namespace Core
{
    public interface IQuery<TQuery, out TResponse> : IRequestBase
        where TQuery : IRequestBase
        where TResponse : IResponseBase
    {
    }
}