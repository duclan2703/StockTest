using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IMediator
    {
        Task<TResponse> SendAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query)
            where TQuery : IRequestBase
            where TResponse : IResponseBase;

        Task<TResponse> SendAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command)
            where TCommand : IRequestBase
            where TResponse : IResponseBase;
    }

}