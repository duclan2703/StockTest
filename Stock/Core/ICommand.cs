using Core.Base;

namespace Core
{
    public interface ICommand<TCommand, out TResponse> : IRequestBase
        where TCommand : IRequestBase
    {
    }
}
