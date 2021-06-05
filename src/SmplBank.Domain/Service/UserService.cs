using SmplBank.Domain.Common;
using SmplBank.Domain.Dto.User;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service
{
    public class UserService : IUserService
    {
        private const string AllowedAccountNumberChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int AccountNumberLength = 12;

        private readonly IUserRepository userRepository;
        private readonly IAccountRepository accountRepository;
        private readonly ISecurityService securityService;

        public UserService(IUserRepository userRepository, IAccountRepository accountRepository, ISecurityService securityService)
        {
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.securityService = securityService;
        }

        public async Task InsertUserAsync(InsertUserDto dto)
        {
            var username = dto.Username.Trim();
            var password = dto.Password.Trim();
            var hashedPassword = this.securityService.HashPassword(password);

            var user = new User
            {
                Username = username,
                Password = hashedPassword
            };

            var userId = await this.userRepository.InsertAsync(user);

            var retryCount = 0;
            string accountNumber = null;

            while (retryCount < 3 && accountNumber == null)
            {
                var draftAccountNumber = RandomString();

                var doesAccountNumberExist = await this.accountRepository.DoesAccountNumberExistAsync(draftAccountNumber);

                if (doesAccountNumberExist)
                    retryCount++;
                else accountNumber = draftAccountNumber;
            }

            if (accountNumber == null)
                throw new InternalErrorDomainException("Generate account number exceeds max retry count.");

            var account = new Account
            {
                AccountNumber = accountNumber,
                UserId = userId
            };

            await this.accountRepository.InsertAsync(account);
        }

        public async Task<User> GetByCredentialAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ValidationDomainException($"{nameof(username)} cannot be empty.");

            if (string.IsNullOrEmpty(password))
                throw new ValidationDomainException($"{nameof(password)} cannot be empty.");

            var user = await this.userRepository.GetByUsernameAsync(username);
            var isValid = user != null && this.securityService.ValidatePassword(password, user.Password);

            return isValid ? user : null;
        }

        // Original source code: https://stackoverflow.com/questions/730268/unique-random-string-generation
        private string RandomString()
        {
            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(AllowedAccountNumberChars).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(string.Format("allowedChars may contain no more than {0} characters.", byteSize));

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < AccountNumberLength)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < AccountNumberLength; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }
    }
}
