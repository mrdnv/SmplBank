namespace SmplBank.Domain.Dto.Transaction
{
    public class TransferTransactionDto
    {
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
