using SmplBank.Application.Requests.Queries;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class GetAllTransactionsHandler : BaseHandler<GetAllTransactionQuery, IEnumerable<TransactionDto>>
    {
        private readonly ITransactionService transactionService;

        public GetAllTransactionsHandler(ITransactionService transactionService) : base()
        {
            this.transactionService = transactionService;
        }

        protected override Task<IEnumerable<TransactionDto>> Process(GetAllTransactionQuery request, CancellationToken cancellationToken)
            => this.transactionService.GetAllTransactionsAsync(request.AccountId);
    }
}
