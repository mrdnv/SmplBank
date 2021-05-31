using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Validation.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmplBank.Domain.Validation
{
    public class TransferTransactionValidator : IValidator<Transaction, TransferTransactionDto>
    {
        private readonly IAccountRepository accountRepository;

        public TransferTransactionValidator(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<IValidatedObject<TransferTransactionDto>> ValidateAsync(TransferTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ValidationDomainException("Amount must be a positive number.");

            var fromAccount = await this.accountRepository.FindAsync(dto.FromAccountId);

            if (fromAccount == null)
                throw new EntityNotFoundDomainException($"From Account does not exist.");

            if (fromAccount.Balance < dto.Amount)
                throw new ValidationDomainException($"Insufficient balance.");

            var toAccount = await this.accountRepository.GetByAccountNumberAsync(dto.ToAccountNumber);

            if (toAccount == null)
                throw new EntityNotFoundDomainException($"To Account does not exist.");

            var validatedObject = new ValidatedObject<TransferTransactionDto>();
            validatedObject.AddFederatedObject(fromAccount, _ => _.FromAccountId);
            validatedObject.AddFederatedObject(toAccount, _ => _.ToAccountNumber);

            return validatedObject;
        }
    }
}
