using SmplBank.Application.Requests.Commands;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class TransferTransactionHandler : BaseHandler<TransferTransactionCommand>
    {
        private readonly ITransactionService transactionService;

        public TransferTransactionHandler(ITransactionService transactionService) : base()
        {
            this.transactionService = transactionService;
        }

        protected override Task Process(TransferTransactionCommand request, CancellationToken cancellationToken)
        {
            var dto = new TransferTransactionDto { Amount = request.Amount, ToAccountNumber = request.ToAccountNumber, FromAccountId = request.AccountId };
            return this.transactionService.TransferAsync(dto);
        }
    }
}
