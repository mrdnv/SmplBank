using MediatR;
using SmplBank.Application.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, Response<TResponse>>
        where TRequest : IRequest<Response<TResponse>>
    {
        protected BaseHandler()
        {
        }

        public async Task<Response<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var responseValue = await Process(request, cancellationToken);

            return Response.Success(responseValue);
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class BaseHandler<TRequest> : IRequestHandler<TRequest, Response>
        where TRequest : IRequest<Response>
    {
        protected BaseHandler()
        {
        }

        public async Task<Response> Handle(TRequest request, CancellationToken cancellationToken)
        {
            await Process(request, cancellationToken);

            return Response.Success();
        }

        protected abstract Task Process(TRequest request, CancellationToken cancellationToken);
    }
}
