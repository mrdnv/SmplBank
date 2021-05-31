using MediatR;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.AccountDto;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class GetAccountInfoHandler : IRequestHandler<GetAccountInfoRequest, AccountDto>
    {
        private readonly IAccountService accountService;

        public GetAccountInfoHandler(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        public Task<AccountDto> Handle(GetAccountInfoRequest request, CancellationToken cancellationToken)
        {
            return this.accountService.FindAsync(request.AccountId);
        }
    }
}
