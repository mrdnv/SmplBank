using SmplBank.Domain.Entity.Enum;
using SmplBank.Domain.Entity.Enums;

namespace SmplBank.Domain.Entity
{
    public class Transaction : Entity
    {
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal? EndBalance { get; set; }
        public int? LinkedTransactionId { get; set; }
    }
}
