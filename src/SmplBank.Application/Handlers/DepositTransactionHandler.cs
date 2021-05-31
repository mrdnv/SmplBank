using MediatR;
using SmplBank.Application.Notifications;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class DepositTransactionHandler : IRequestHandler<DepositTransactionRequest>
    {
        private readonly IMediator mediator;
        private readonly ITransactionService transactionService;

        public DepositTransactionHandler(IMediator mediator, ITransactionService transactionService)
        {
            this.mediator = mediator;
            this.transactionService = transactionService;
        }

        public async Task<Unit> Handle(DepositTransactionRequest request, CancellationToken cancellationToken)
        {
            var dto = new DepositTransactionDto { Amount = request.Amount, AccountId = request.AccountId };
            var transactionId = await this.transactionService.DepositAsync(dto);
            await this.mediator.Publish(new TransactionCreatedNotification { TransactionId = transactionId });

            return Unit.Value;
        }
    }
}
