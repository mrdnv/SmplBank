using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SmplBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IMediator mediator;

        public BaseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected T CreateAnonymousRequest<T>() where T : IRequest, new()
        {
            var request = new T();

            return request;
        }
    }
}
