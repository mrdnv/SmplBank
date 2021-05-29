using Moq;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service;
using SmplBank.Domain.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SmplBank.Domain.Test
{
    public class AccountServiceTest
    {
        private Mock<IAccountRepository> mockAccountRepository;
        private IAccountService accountService;

        public AccountServiceTest()
        {
            this.mockAccountRepository = new Mock<IAccountRepository>();
            this.accountService = new AccountService(mockAccountRepository.Object);
        }

        [Fact]
        public async Task Verify_FindAsync_ShouldReturnData()
        {
            // Arrange
            var expectedAccountNumber = "123456789ABC";
            decimal expectedBalance = 345;
            var expectedRegisteredOn = DateTime.Now.AddMonths(-1);

            var account = new Account
            {
                AccountNumber = expectedAccountNumber,
                Balance = expectedBalance,
                CreatedOn = expectedRegisteredOn
            };

            this.mockAccountRepository.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(account);

            // Action
            var actual = await this.accountService.FindAsync(1);

            // Assert
            Assert.Equal(expectedAccountNumber, actual.AccountNumber);
            Assert.Equal(expectedBalance, actual.Balance);
            Assert.Equal(expectedRegisteredOn, actual.RegisteredOn);
        }

        [Fact]
        public async Task Verify_FindAsync_NotFound()
        {
            // Arrange
            this.mockAccountRepository.Setup(_ => _.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Account);

            // Action
            Task action() => this.accountService.FindAsync(1);

            // Assert

            await Assert.ThrowsAsync<EntityNotFoundDomainException>(action);
        }

        [Fact]
        public async Task Verify_GetByUserIdAsync_ShouldReturnData()
        {
            // Arrange
            var expectedAccountNumber = "123456789ABC";
            decimal expectedBalance = 345;
            var expectedRegisteredOn = DateTime.Now.AddMonths(-1);

            var expected = new List<Account>
            {
                new Account
                {
                    AccountNumber = expectedAccountNumber,
                    Balance = expectedBalance,
                    CreatedOn = expectedRegisteredOn
                }
            };

            this.mockAccountRepository.Setup(_ => _.GetByUserId(It.IsAny<int>()))
                .ReturnsAsync(expected);

            // Action
            var actual = await this.accountService.GetByUserIdAsync(1);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
