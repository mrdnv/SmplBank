using SmplBank.Application.Requests.Queries;
using SmplBank.Domain.Dto.AccountDto;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class GetAccountInfoHandler : BaseHandler<GetAccountInfoQuery, AccountDto>
    {
        private readonly IAccountService accountService;

        public GetAccountInfoHandler(IAccountService accountService) : base()
        {
            this.accountService = accountService;
        }

        protected override Task<AccountDto> Process(GetAccountInfoQuery request, CancellationToken cancellationToken)
            => this.accountService.FindAsync(request.AccountId);
    }
}
