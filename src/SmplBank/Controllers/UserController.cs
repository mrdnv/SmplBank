using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests;
using SmplBank.Filters;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("register")]
        [Transaction]
        public async Task<IActionResult> InsertUserAsync(InsertUseRequest request)
        {
            await this.mediator.Send(request);

            return Ok();
        }
    }
}
