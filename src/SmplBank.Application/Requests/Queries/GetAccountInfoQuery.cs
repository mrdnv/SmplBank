using SmplBank.Domain.Dto.AccountDto;

namespace SmplBank.Application.Requests.Queries
{
    public record GetAccountInfoQuery : AuthorizedRequest<AccountDto>
    {
    }
}
