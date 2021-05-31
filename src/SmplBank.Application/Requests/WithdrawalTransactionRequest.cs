namespace SmplBank.Application.Requests
{
    public record WithdrawalTransactionRequest : AuthorizedRequest
    {
        public decimal Amount { get; set; }
    }
}
