using SmplBank.Domain.Entity;
using SmplBank.Domain.Repository;
using SmplBank.Infrastructure.Exception;
using SmplBank.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace SmplBank.Infrastructure.ConcurrencyTest
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var connectionString = @"Data Source=localhost;Initial Catalog=SmplBank;Integrated Security=False;User Id=sa;Password=sa;MultipleActiveResultSets=True";
            var sqlConnection = new SqlConnection(connectionString);
            IAccountRepository accountRepository = new AccountRepository(sqlConnection);
            IUserRepository userRepository = new UserRepository(sqlConnection);


            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var userTest = new User
                {
                    Username = "testusername",
                    Password = "testpassword"
                };

                var userTestId = await userRepository.InsertAsync(userTest);

                var accountTest = new Account
                {
                    AccountNumber = "AccountTest1",
                    UserId = userTestId,
                    Balance = 1000
                };

                var accountTestId = await accountRepository.InsertAsync(accountTest);
                string taskName = "";

                try
                {
                    var testData = await accountRepository.FindAsync(accountTestId);
                    // This will update the record successfully.
                    taskName = "task1";
                    await ExecuteUpdateAccountAsync(connectionString, testData, 500, "task1");

                    // This will be failed because the rowversion was already changed.
                    taskName = "task2";
                    await ExecuteUpdateAccountAsync(connectionString, testData, -500, "task2");

                    transactionScope.Complete();
                }
                catch (ConcurrentException ex)
                {
                    Console.WriteLine($"{taskName} failed to update Account due to concurrenct exception.");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static Task ExecuteUpdateAccountAsync(string connectionString, Account account, decimal addBalanceAmount, string taskName)
        {
            var sqlConnection = new SqlConnection(connectionString);
            IAccountRepository accountRepository = new AccountRepository(sqlConnection);
            account.Balance += addBalanceAmount;
            return accountRepository.UpdateAsync(account);
        }
    }
}
