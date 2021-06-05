using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly ISender sender;

        protected BaseController(ISender sender)
        {
            this.sender = sender;
        }

        protected Task SendAnonymousAsync<TRequest>(CancellationToken cancellationToken = default) 
            where TRequest : BaseRequest, new()
            => this.SendAsync(new TRequest(), cancellationToken);

        protected Task<IActionResult> SendAnonymousAsync<TRequest, TResponse>(CancellationToken cancellationToken = default) 
            where TRequest : BaseRequest<TResponse>, new()
            => this.SendAsync(new TRequest(), cancellationToken);

        protected Task<IActionResult> SendAsync<TResponse>(BaseRequest<TResponse> request, CancellationToken cancellationToken = default)
            => this.SendAsync(request, (value) => Ok(value), cancellationToken);

        protected async Task<IActionResult> SendAsync<TResponse>(BaseRequest<TResponse> request, Func<TResponse, IActionResult> onSuccess, CancellationToken cancellationToken = default)
        {
            var response = await this.sender.Send(request, cancellationToken);

            if (!response.IsValid)
            {
                return BadRequest(response.ErrorMessages);
            }

            return onSuccess(response.Value);
        }

        protected async Task<IActionResult> SendAsync(BaseRequest request, CancellationToken cancellationToken = default)
        {
            var response = await this.sender.Send(request, cancellationToken);

            if (!response.IsValid)
            {
                return BadRequest(response.ErrorMessages);
            }

            return Ok();
        }
    }
}
