namespace SmplBank.Application.Requests
{
    public class WithdrawalTransactionRequest : AuthorizedRequest
    {
        public decimal Amount { get; set; }
    }
}
