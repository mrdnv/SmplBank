using SmplBank.Domain.Dto.Transaction;
using System.Collections.Generic;

namespace SmplBank.Application.Requests.Queries
{
    public record GetAllTransactionQuery : AuthorizedRequest<IEnumerable<TransactionDto>>
    {
    }
}
