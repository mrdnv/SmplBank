using MediatR;
using SmplBank.Application.Notifications;
using SmplBank.Application.Requests.Commands;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class DepositTransactionHandler : BaseHandler<DepositTransactionCommand>
    {
        private readonly IPublisher publisher;
        private readonly ITransactionService transactionService;

        public DepositTransactionHandler(IPublisher publisher, ITransactionService transactionService) : base()
        {
            this.publisher = publisher;
            this.transactionService = transactionService;
        }

        protected async override Task Process(DepositTransactionCommand request, CancellationToken cancellationToken)
        {
            var dto = new DepositTransactionDto { Amount = request.Amount, AccountId = request.AccountId };
            var transactionId = await this.transactionService.DepositAsync(dto);
            await this.publisher.Publish(new TransactionCreatedNotification { TransactionId = transactionId });
        }
    }
}
