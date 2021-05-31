using MediatR;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class DepositTransactionHandler : IRequestHandler<DepositTransactionRequest>
    {
        private readonly ITransactionService transactionService;

        public DepositTransactionHandler(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public async Task<Unit> Handle(DepositTransactionRequest request, CancellationToken cancellationToken)
        {
            var dto = new DepositTransactionDto { Amount = request.Amount, AccountId = request.AccountId };
            await this.transactionService.DepositAsync(dto);

            return Unit.Value;
        }
    }
}
