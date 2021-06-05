using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests.Commands;
using SmplBank.Filters;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ISender sender) : base(sender)
        {
        }

        [HttpPost("register")]
        [Transaction]
        public Task<IActionResult> InsertUserAsync(InsertUserCommand request) => this.SendAsync(request);
    }
}
