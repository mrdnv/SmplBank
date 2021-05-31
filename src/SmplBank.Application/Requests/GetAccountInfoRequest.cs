using SmplBank.Domain.Dto.AccountDto;

namespace SmplBank.Application.Requests
{
    public record GetAccountInfoRequest : AuthorizedRequest<AccountDto>
    {
    }
}
