namespace SmplBank.Domain.Dto.Transaction
{
    public class TransferTransactionDto
    {
        internal int FromAccountId { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
