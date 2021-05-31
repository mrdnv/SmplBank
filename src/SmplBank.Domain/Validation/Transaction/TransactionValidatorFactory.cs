using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Validation.Interfaces;

namespace SmplBank.Domain.Validation.Transaction
{
    public class TransactionValidatorFactory : AbstractValidatorFactory<Entity.Transaction>
    {
        public TransactionValidatorFactory(
            IValidator<Entity.Transaction, DepositTransactionDto> depositTransactionDto,
            IValidator<Entity.Transaction, TransferTransactionDto> transferTransactionDto,
            IValidator<Entity.Transaction, WithdrawalTransactionDto> withdrawalTransactionDto)
            : base(depositTransactionDto, transferTransactionDto, withdrawalTransactionDto)
        {
        }
    }
}
