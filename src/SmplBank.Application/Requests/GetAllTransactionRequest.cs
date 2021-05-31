using SmplBank.Domain.Dto.Transaction;
using System.Collections.Generic;

namespace SmplBank.Application.Requests
{
    public record GetAllTransactionRequest : AuthorizedRequest<IEnumerable<TransactionDto>>
    {
    }
}
