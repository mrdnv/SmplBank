using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Validation.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmplBank.Domain.Validation.Transaction
{
    public class DepositTransactionValidator : IValidator<Entity.Transaction, DepositTransactionDto>
    {
        private readonly IAccountRepository accountRepository;

        public DepositTransactionValidator(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<IValidatedObject<DepositTransactionDto>> ValidateAsync(DepositTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ValidationDomainException("Amount must be a positive number.");

            var account = await accountRepository.FindAsync(dto.AccountId);

            if (account == null)
                throw new EntityNotFoundDomainException($"Account does not exist.");

            var validatedObject = new ValidatedObject<DepositTransactionDto>();
            validatedObject.AddFederatedObject(account, _ => _.AccountId);

            return validatedObject;
        }
    }
}