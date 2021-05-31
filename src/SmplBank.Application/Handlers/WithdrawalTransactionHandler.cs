using MediatR;
using SmplBank.Application.Notifications;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class WithdrawalTransactionHandler : IRequestHandler<WithdrawalTransactionRequest>
    {
        private readonly IMediator mediator;
        private readonly ITransactionService transactionService;

        public WithdrawalTransactionHandler(IMediator mediator, ITransactionService transactionService)
        {
            this.mediator = mediator;
            this.transactionService = transactionService;
        }

        public async Task<Unit> Handle(WithdrawalTransactionRequest request, CancellationToken cancellationToken)
        {
            var dto = new WithdrawalTransactionDto { Amount = request.Amount, AccountId = request.AccountId };
            var transactionId = await this.transactionService.WithdrawAsync(dto);
            await this.mediator.Publish(new TransactionCreatedNotification { TransactionId = transactionId });

            return Unit.Value;
        }
    }
}
