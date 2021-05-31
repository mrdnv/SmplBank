using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Validation.Interfaces;

namespace SmplBank.Domain.Validation
{
    public class TransactionValidatorFactory : AbstractValidatorFactory<Transaction>
    {
        public TransactionValidatorFactory(
            IValidator<Transaction, DepositTransactionDto> depositTransactionDto, 
            IValidator<Transaction, TransferTransactionDto> transferTransactionDto,
            IValidator<Transaction, WithdrawalTransactionDto> withdrawalTransactionDto) 
            : base(depositTransactionDto, transferTransactionDto, withdrawalTransactionDto)
        {
        }
    }
}
