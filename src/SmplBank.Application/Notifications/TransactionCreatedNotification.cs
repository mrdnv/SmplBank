using MediatR;

namespace SmplBank.Application.Notifications
{
    public record TransactionCreatedNotification : INotification
    {
        public int TransactionId { get; init; }
    }
}
