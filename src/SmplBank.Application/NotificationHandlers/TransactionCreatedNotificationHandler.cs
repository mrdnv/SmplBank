using MediatR;
using SmplBank.Application.Notifications;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.NotificationHandlers
{
    public class TransactionCreatedNotificationHandler : INotificationHandler<TransactionCreatedNotification>
    {
        private readonly ITransactionService transactionService;

        public TransactionCreatedNotificationHandler(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public Task Handle(TransactionCreatedNotification notification, CancellationToken cancellationToken)
            => this.transactionService.ProcessTransaction(notification.TransactionId);
    }
}
