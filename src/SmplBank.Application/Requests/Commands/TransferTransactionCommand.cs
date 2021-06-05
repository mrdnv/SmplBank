namespace SmplBank.Application.Requests.Commands
{
    public record TransferTransactionCommand : AuthorizedRequest
    {
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
