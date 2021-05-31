using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator mediator;

        protected BaseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected Task SendAnonymousAsync<TRequest>(CancellationToken cancellationToken = default) where TRequest : IRequest, new()
            => this.SendAsync(new TRequest(), cancellationToken);

        protected Task<TResponse> SendAnonymousAsync<TRequest, TResponse>(CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>, new()
            => this.SendAsync(new TRequest(), cancellationToken);

        protected Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            => this.mediator.Send(request, cancellationToken);
    }
}
