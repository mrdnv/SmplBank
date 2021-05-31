using MediatR;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmplBank.Application.Handlers
{
    public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionRequest, IEnumerable<TransactionDto>>
    {
        private readonly ITransactionService transactionService;

        public GetAllTransactionsHandler(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public Task<IEnumerable<TransactionDto>> Handle(GetAllTransactionRequest request, CancellationToken cancellationToken)
            => this.transactionService.GetAllTransactionsAsync(request.AccountId);
    }
}
