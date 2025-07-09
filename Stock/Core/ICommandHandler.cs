using Core.Base;

namespace Core
{
    public interface ICommandHandler<TCommand, TResult> : IHandlerBase<TResult, TCommand>
        where TCommand : ICommand<TCommand, TResult>
        where TResult : IResponseBase
    {
    }
}
