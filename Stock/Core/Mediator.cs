using Core.Base;
using Core.Exception;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Threading;

namespace Core
{
    public class Mediator : IMediator
    {
        class RequestType
        {
            public required Type HandlerType { get; set; }

            public required Type ValidatorType { get; set; }
        }

        static ConcurrentDictionary<Type, Lazy<RequestType>> _requestTypeMap =
            new ConcurrentDictionary<Type, Lazy<RequestType>>();

        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> SendAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query)
            where TQuery : IRequestBase
            where TResponse : IResponseBase
        {
            return await ExecuteQuery<TQuery, TResponse>((TQuery)query);
        }

        public async Task<TResponse> SendAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command)
            where TCommand : IRequestBase
            where TResponse : IResponseBase
        {
            return await ExecuteCommand<TCommand, TResponse>((TCommand)command);
        }

        private async Task<TResult> ExecuteQuery<TQuery, TResult>(TQuery query)
            where TQuery : IRequestBase
            where TResult : IResponseBase
        {
            var queryType = GetRequestType<TQuery, IQuery<TQuery, TResult>, TResult>();
            return await ExecuteRequest<TResult, TQuery>(query, queryType);
        }

        private async Task<TResult> ExecuteCommand<TCommand, TResult>(TCommand command)
            where TCommand : IRequestBase
            where TResult : IResponseBase
        {
            var commandType = GetRequestType<TCommand, ICommand<TCommand, TResult>, TResult>();
            return await ExecuteRequest<TResult, TCommand>(command, commandType);
        }

        private static RequestType GetRequestType<TRequest, ITRequest, TResult>()
            where TRequest : IRequestBase
            where ITRequest : IRequestBase
            where TResult : IResponseBase
        {
            var request = typeof(TRequest);
            if (!_requestTypeMap.ContainsKey(request))
            {
                _requestTypeMap.TryAdd(request, new Lazy<RequestType>(() =>
                {
                    return Mediator.CreateRequestType<TRequest, ITRequest, TResult>();
                }));
            }

            var requestType = _requestTypeMap[request].Value;
            return requestType;
        }

        private static RequestType CreateRequestType<TRequest, ITRequest, TResult>()
            where TRequest : IRequestBase
            where TResult : IResponseBase
        {
            Type interfaceRequestType = typeof(ITRequest);

            var requestType = typeof(TRequest);

            Type interfaceQueryType = typeof(IQuery<TRequest, TResult>);
            Type interfaceCommandType = typeof(ICommand<TRequest, TResult>);
            Type handlerGenericType = interfaceRequestType == interfaceQueryType ? typeof(IQueryHandler<,>) :
                interfaceRequestType == interfaceCommandType ? typeof(ICommandHandler<,>) :
                throw new NotSupportedException();

            Type handler = handlerGenericType.MakeGenericType(requestType, typeof(TResult));


            Type validatorGenericType = typeof(IValidator<>);
            Type validatorType = validatorGenericType.MakeGenericType(requestType);

            return new RequestType()
            {
                HandlerType = handler,
                ValidatorType = validatorType
            };
        }

        private async Task<TResult> ExecuteRequest<TResult, TRequest>(TRequest query, RequestType queryType)
            where TRequest : IRequestBase
            where TResult : IResponseBase
        {
            try
            {
                await ValidateRequest(query, queryType.ValidatorType);

                var handler = (IHandlerBase<TResult, TRequest>)_serviceProvider.GetService(queryType.HandlerType)!;
                var results = await handler.Handle(query);

                return results;
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        private async Task ValidateRequest<TQuery>(TQuery query, Type validatorType)
        {
            var validator = (IValidator<TQuery>)_serviceProvider.GetService(validatorType)!;
            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(query);

                if (!validationResult.IsValid)
                {
                    throw new InvalidRequestException($"Error: {string.Join(", ", validationResult.Errors.Select(i => i.ErrorMessage).Distinct())}");
                }
            }
        }
    }
}
