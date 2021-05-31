using SmplBank.Domain.Dto.Transaction;
using System.Collections.Generic;

namespace SmplBank.Application.Requests
{
    public class GetAllTransactionRequest : AuthorizedRequest<IEnumerable<TransactionDto>>
    {
    }
}
