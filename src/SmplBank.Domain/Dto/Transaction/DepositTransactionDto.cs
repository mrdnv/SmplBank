namespace SmplBank.Domain.Dto.Transaction
{
    public record DepositTransactionDto
    {
        public decimal Amount { get; set; }

        public int AccountId { get; set; }
    }
}
