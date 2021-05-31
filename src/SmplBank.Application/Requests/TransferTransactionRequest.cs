namespace SmplBank.Application.Requests
{
    public class TransferTransactionRequest : AuthorizedRequest
    {
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
