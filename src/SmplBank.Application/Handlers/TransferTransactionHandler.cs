using MediatR;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class TransferTransactionHandler : IRequestHandler<TransferTransactionRequest>
    {
        private readonly ITransactionService transactionService;

        public TransferTransactionHandler(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public async Task<Unit> Handle(TransferTransactionRequest request, CancellationToken cancellationToken)
        {
            var dto = new TransferTransactionDto { Amount = request.Amount, ToAccountNumber = request.ToAccountNumber };
            await this.transactionService.TransferAsync(request.AccountId, dto);

            return Unit.Value;
        }
    }
}
