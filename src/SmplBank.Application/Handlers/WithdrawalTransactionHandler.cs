using MediatR;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class WithdrawalTransactionHandler : IRequestHandler<WithdrawalTransactionRequest>
    {
        private readonly ITransactionService transactionService;

        public WithdrawalTransactionHandler(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public async Task<Unit> Handle(WithdrawalTransactionRequest request, CancellationToken cancellationToken)
        {
            var dto = new WithdrawalTransactionDto { Amount = request.Amount };
            await this.transactionService.WithdrawAsync(request.AccountId, dto);

            return Unit.Value;
        }
    }
}
