namespace SmplBank.Domain.Dto.Transaction
{
    public class WithdrawalTransactionDto
    {
        internal int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
