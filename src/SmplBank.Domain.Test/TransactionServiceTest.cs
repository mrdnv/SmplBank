using Moq;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service;
using SmplBank.Domain.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmplBank.Domain.Test
{
    public class TransactionServiceTest
    {
        private readonly Mock<IAccountRepository> mockAccountRepository;
        private readonly Mock<ITransactionRepository> mockTransactionRepository;
        private readonly ITransactionService transactionService;

        public TransactionServiceTest()
        {
            this.mockAccountRepository = new Mock<IAccountRepository>();
            this.mockTransactionRepository = new Mock<ITransactionRepository>();
            this.transactionService = new TransactionService(mockAccountRepository.Object, mockTransactionRepository.Object);
        }

        public static IEnumerable<object[]> TestDepositDataException = new[]
        {
            new object []
            {
                1,
                null as DepositTransactionDto,
                null as Account,
                typeof(ArgumentNullException)
            },
            new object []
            {
                1,
                new DepositTransactionDto
                {
                    Amount = -1
                },
                null as Account,
                typeof(ValidationDomainException)
            },
            new object []
            {
                1,
                new DepositTransactionDto
                {
                    Amount = 100
                },
                null as Account,
                typeof(EntityNotFoundDomainException)
            },
        };

        public static IEnumerable<object[]> TestTransferDataException = new[]
        {
            new object[]
            {
                1,
                null as Account,
                null as TransferTransactionDto,
                null as Account,
                typeof(ArgumentNullException)
            },
            new object[]
            {
                1,
                null as Account,
                new TransferTransactionDto { Amount = -1, ToAccountNumber = "AccountNumber" },
                null as Account,
                typeof(ValidationDomainException)
            },
            new object[]
            {
                1,
                null as Account,
                new TransferTransactionDto { Amount = 100, ToAccountNumber = "AccountNumber" },
                null as Account,
                typeof(EntityNotFoundDomainException)
            },
            new object[]
            {
                1,
                new Account
                {
                    Id = 1,
                    Balance = 50
                },
                new TransferTransactionDto { Amount = 100, ToAccountNumber = "AccountNumber" },
                null as Account,
                typeof(ValidationDomainException)
            },
            new object[]
            {
                1,
                new Account
                {
                    Id = 1,
                    Balance = 150
                },
                new TransferTransactionDto { Amount = 100, ToAccountNumber = "AccountNumber" },
                null as Account,
                typeof(EntityNotFoundDomainException)
            },
        };

        public static IEnumerable<object[]> TestWithdrawDataException = new[]
        {
            new object []
            {
                1,
                null as WithdrawalTransactionDto,
                null as Account,
                typeof(ArgumentNullException)
            },
            new object []
            {
                1,
                new WithdrawalTransactionDto
                {
                    Amount = -1
                },
                null as Account,
                typeof(ValidationDomainException)
            },
            new object []
            {
                1,
                new WithdrawalTransactionDto
                {
                    Amount = 100
                },
                null as Account,
                typeof(EntityNotFoundDomainException)
            },
            new object []
            {
                1,
                new WithdrawalTransactionDto
                {
                    Amount = 100
                },
                new Account
                { 
                    Balance = 50
                },
                typeof(ValidationDomainException)
            },
        };

        [Theory]
        [MemberData(nameof(TestDepositDataException))]
        public async Task Verify_DepositAsync_ShouldThrowException(int accountId, DepositTransactionDto dto, Account account, Type expectedException)
        {
            // Arrange
            this.mockAccountRepository.Setup(_ => _.FindAsync(accountId)).ReturnsAsync(account);

            // Action
            Task action() => this.transactionService.DepositAsync(accountId, dto);

            // Arrange
            await Assert.ThrowsAsync(expectedException, action);
        }

        [Theory]
        [MemberData(nameof(TestTransferDataException))]
        public async Task Verify_TransferAsync_ShouldThrowException(int accountId, Account fromAccount, TransferTransactionDto dto, Account toAccount, Type expectedException)
        {
            // Arrange
            this.mockAccountRepository.Setup(_ => _.FindAsync(accountId))
                .ReturnsAsync(fromAccount);

            if (dto != null)
            {
                this.mockAccountRepository.Setup(_ => _.GetByAccountNumberAsync(dto.ToAccountNumber))
                    .ReturnsAsync(toAccount);
            }

            // Action
            Task action() => this.transactionService.TransferAsync(accountId, dto);

            // Arrange
            await Assert.ThrowsAsync(expectedException, action);
        }

        [Theory]
        [MemberData(nameof(TestWithdrawDataException))]
        public async Task Verify_WithdrawAsync_ShouldThrowException(int accountId, WithdrawalTransactionDto dto, Account account, Type expectedException)
        {
            // Arrange
            this.mockAccountRepository.Setup(_ => _.FindAsync(accountId))
                .ReturnsAsync(account);

            // Action
            Task action() => this.transactionService.WithdrawAsync(accountId, dto);

            // Arrange
            await Assert.ThrowsAsync(expectedException, action);
        }

        [Fact]
        public async Task Verify_DepositAsync_Successfully()
        {
            // Arrange
            this.mockAccountRepository.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(new Account
                {
                    Id = 1,
                    Balance = 100,
                    CreatedOn = DateTime.Now.AddMonths(-1),
                    UserId = 1
                });

            var dto = new DepositTransactionDto
            {
                Amount = 200
            };

            // Action
            await this.transactionService.DepositAsync(1, dto);

            // Assert
            this.mockAccountRepository.Verify(_ => _.UpdateAsync(It.IsAny<Account>()), Times.Once);
            this.mockTransactionRepository.Verify(_ => _.InsertAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task Verify_GetAllTransactionAsync_Successfully()
        {
            // Arrange
            var expectedTransactions = new List<TransactionDto>
            {
                new TransactionDto
                {
                    Amount = 100,
                    TransactionDate = DateTime.Now.AddDays(-3)
                },
                new TransactionDto
                {
                    Amount = 100,
                    TransactionDate = DateTime.Now.AddDays(-5)
                },
                new TransactionDto
                {
                    Amount = 100,
                    TransactionDate = DateTime.Now.AddDays(-1)
                },
            };

            this.mockTransactionRepository.Setup(_ => _.GetTransactionsByAccountId(It.IsAny<int>()))
                .ReturnsAsync(expectedTransactions);

            // Action
            var actualTransactions = await this.transactionService.GetAllTransactionsAsync(1);

            // Assert
            Assert.Equal(expectedTransactions.OrderByDescending(_ => _.TransactionDate), actualTransactions);
        }

        [Fact]
        public async Task Verify_TransferAsync_Successfully()
        {
            // Arrange
            decimal expectedPostBalance = 500;
            var fromAccount = new Account
            {
                AccountNumber = "AccountNumber1",
                Id = 1,
                Balance = 1000,
                UserId = 1
            };

            var dto = new TransferTransactionDto
            {
                Amount = 500,
                ToAccountNumber = "AccountNumber2"
            };

            this.mockAccountRepository.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(fromAccount);
            this.mockAccountRepository.Setup(_ => _.GetByAccountNumberAsync(dto.ToAccountNumber))
                .ReturnsAsync(new Account { AccountNumber = dto.ToAccountNumber, Id = 2, UserId = 2 });
            this.mockTransactionRepository.Setup(_ => _.InsertAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(1);
            this.mockTransactionRepository.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(new Transaction());

            // Action
            await this.transactionService.TransferAsync(1, dto);

            // Assert
            Assert.Equal(expectedPostBalance, fromAccount.Balance);
            this.mockTransactionRepository.Verify(_ => _.InsertAsync(It.IsAny<Transaction>()), Times.Exactly(2));
            this.mockTransactionRepository.Verify(_ => _.UpdateAsync(It.IsAny<Transaction>()), Times.Once);
            this.mockAccountRepository.Verify(_ => _.UpdateAsync(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public async Task Verify_WithdrawAsync_Successfully()
        {
            // Arrange
            decimal preBalance = 300;
            decimal postBalance = 100;

            var account = new Account
            {
                Id = 1,
                Balance = preBalance,
                CreatedOn = DateTime.Now.AddMonths(-1),
                UserId = 1
            };

            var dto = new WithdrawalTransactionDto
            {
                Amount = 200
            };

            this.mockAccountRepository.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(account);

            // Action
            await this.transactionService.WithdrawAsync(1, dto);

            // Assert
            Assert.Equal(postBalance, account.Balance);
            this.mockAccountRepository.Verify(_ => _.UpdateAsync(It.IsAny<Account>()), Times.Once);
            this.mockTransactionRepository.Verify(_ => _.InsertAsync(It.IsAny<Transaction>()), Times.Once);
        }
    }
}
