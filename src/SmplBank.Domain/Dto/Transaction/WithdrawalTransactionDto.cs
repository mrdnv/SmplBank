namespace SmplBank.Domain.Dto.Transaction
{
    public record WithdrawalTransactionDto
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
