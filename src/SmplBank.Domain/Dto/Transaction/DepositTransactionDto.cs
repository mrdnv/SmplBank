namespace SmplBank.Domain.Dto.Transaction
{
    public class DepositTransactionDto
    {
        public decimal Amount { get; set; }

        internal int AccountId { get; set; }
    }
}
