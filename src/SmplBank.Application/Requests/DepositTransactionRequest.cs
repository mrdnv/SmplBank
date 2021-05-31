namespace SmplBank.Application.Requests
{
    public class DepositTransactionRequest : AuthorizedRequest
    {
        public decimal Amount { get; set; }
    }
}
