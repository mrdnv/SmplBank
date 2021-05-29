using SmplBank.Domain.Entity.Enum;
using SmplBank.Domain.Entity.Enums;
using System;

namespace SmplBank.Domain.Dto.Transaction
{
    public class TransactionDto
    {
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionTypeName { get => TransactionType.ToString(); }
        public TransactionStatus TransactionStatus { get; set; }
        public string TransactionStatusName { get => TransactionStatus.ToString(); }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? EndBalance { get; set; }
        public string Description { get; set; }
    }
}
