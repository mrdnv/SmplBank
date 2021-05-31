using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Validation.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmplBank.Domain.Validation.Transaction
{
    public class WithdrawalTransactionValidator : IValidator<Entity.Transaction, WithdrawalTransactionDto>
    {
        private readonly IAccountRepository accountRepository;

        public WithdrawalTransactionValidator(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<IValidatedObject<WithdrawalTransactionDto>> ValidateAsync(WithdrawalTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ValidationDomainException("Amount must be a positive number.");

            var account = await accountRepository.FindAsync(dto.AccountId);

            if (account == null)
                throw new EntityNotFoundDomainException($"Account does not exist.");

            if (account.Balance < dto.Amount)
                throw new ValidationDomainException($"Insufficient balance.");

            var validatedObject = new ValidatedObject<WithdrawalTransactionDto>();
            validatedObject.AddFederatedObject(account, _ => _.AccountId);

            return validatedObject;
        }
    }
}
