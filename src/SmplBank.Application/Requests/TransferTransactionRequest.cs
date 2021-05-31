namespace SmplBank.Application.Requests
{
    public record TransferTransactionRequest : AuthorizedRequest
    {
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
