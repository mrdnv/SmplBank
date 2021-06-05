namespace SmplBank.Application.Requests.Commands
{
    public record DepositTransactionCommand : AuthorizedRequest
    {
        public decimal Amount { get; set; }
    }
}
