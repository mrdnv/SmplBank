namespace SmplBank.Application.Requests
{
    public record DepositTransactionRequest : AuthorizedRequest
    {
        public decimal Amount { get; set; }
    }
}
