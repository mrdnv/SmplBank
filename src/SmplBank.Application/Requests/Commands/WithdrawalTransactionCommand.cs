namespace SmplBank.Application.Requests.Commands
{
    public record WithdrawalTransactionCommand : AuthorizedRequest
    {
        public decimal Amount { get; set; }
    }
}
