namespace SmplBank.Domain.Dto.Transaction
{
    public record TransferTransactionDto
    {
        public int FromAccountId { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
