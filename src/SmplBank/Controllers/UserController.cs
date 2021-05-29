using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Domain.Dto.User;
using SmplBank.Domain.Service.Interface;
using SmplBank.Filters;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        [Transaction]
        [AllowAnonymous]
        public async Task<IActionResult> InsertUserAsync(InsertUserDto dto)
        {
            await this.userService.InsertUserAsync(dto);

            return Ok();
        }
    }
}
