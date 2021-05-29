using Moq;
using SmplBank.Domain.Common;
using SmplBank.Domain.Dto.User;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service;
using SmplBank.Domain.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmplBank.Domain.Test
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly Mock<IAccountRepository> mockAccountRepository;
        private readonly Mock<ISecurityService> mockSecurityService;
        private readonly IUserService userService;

        public UserServiceTest()
        {
            this.mockUserRepository = new Mock<IUserRepository>();
            this.mockAccountRepository = new Mock<IAccountRepository>();
            this.mockSecurityService = new Mock<ISecurityService>();
            this.userService = new UserService(mockUserRepository.Object, mockAccountRepository.Object, mockSecurityService.Object);
        }

        public static IEnumerable<object[]> TestInsertUserData = new[]
        {
            new object[]
            {
                null as InsertUserDto,
                true,
                false,
                typeof(ArgumentNullException)
            },
            new object[]
            {
                new InsertUserDto(),
                true,
                false,
                typeof(ValidationDomainException)
            },
            new object[]
            {
                new InsertUserDto { Username = "username" },
                true,
                false,
                typeof(ValidationDomainException)
            },
            new object[]
            {
                new InsertUserDto { Username = "username", Password = "password" },
                true,
                false,
                typeof(DuplicationDomainException)
            },
            new object[]
            {
                new InsertUserDto { Username = "username", Password = "password" },
                false,
                true,
                typeof(InternalErrorDomainException)
            },
            new object[]
            {
                new InsertUserDto { Username = "username", Password = "password" },
                false,
                false
            },
        };

        public static IEnumerable<object[]> TestGetByCredentialData = new[]
        {
            new object[]
            {
                null as string,
                null as string,
                null as User,
                false,
                false,
                typeof(ValidationDomainException)
            },
            new object[]
            {
                "username",
                null as string,
                null as User,
                false,
                false,
                typeof(ValidationDomainException)
            },
            new object[]
            {
                "username",
                "password",
                null as User,
                false,
                false
            },
            new object[]
            {
                "username",
                "password",
                new User { },
                false,
                false
            },
            new object[]
            {
                "username",
                "password",
                new User { },
                true,
                true
            }
        };

        [Theory]
        [MemberData(nameof(TestInsertUserData))]
        public async Task Verify_InsertUser(InsertUserDto dto, bool doesUserExist, bool doesAccountNumberExist, Type expectedException = null)
        {
            // Arrange
            this.mockUserRepository.Setup(_ => _.DoesUsernameExistAsync(It.IsAny<string>()))
                .ReturnsAsync(doesUserExist);

            this.mockUserRepository.Setup(_ => _.InsertAsync(It.IsAny<User>()))
                .ReturnsAsync(1)
                .Verifiable();

            this.mockAccountRepository.Setup(_ => _.DoesAccountNumberExistAsync(It.IsAny<string>()))
                .ReturnsAsync(doesAccountNumberExist);

            this.mockAccountRepository.Setup(_ => _.InsertAsync(It.IsAny<Account>()))
                .ReturnsAsync(1)
                .Verifiable();

            this.mockSecurityService.Setup(_ => _.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword");

            // Action
            Task action() => this.userService.InsertUserAsync(dto);

            // Assert
            if (expectedException != null)
                await Assert.ThrowsAsync(expectedException, action);
            else
            {
                await action();
                this.mockUserRepository.Verify(_ => _.InsertAsync(It.IsAny<User>()), Times.Once);
                this.mockAccountRepository.Verify(_ => _.InsertAsync(It.IsAny<Account>()), Times.Once);
            }
        }

        [Theory]
        [MemberData(nameof(TestGetByCredentialData))]
        public async Task Verify_GetByCredential(string username, string password, User expectedUser, bool isPasswordValid, bool isUserFound, Type expectedException = null)
        {
            // Arrange
            this.mockUserRepository.Setup(_ => _.GetByUsernameAsync(username))
                .ReturnsAsync(expectedUser);
            this.mockSecurityService.Setup(_ => _.ValidatePassword(password, It.IsAny<string>()))
                .Returns(isPasswordValid);

            // Action
            Task<User> action() => this.userService.GetByCredentialAsync(username, password);

            // Arrange
            if (expectedException != null)
                await Assert.ThrowsAsync(expectedException, action);
            else
            {
                var user = await action();
                Assert.Equal(isUserFound, user != null);
            }
        }
    }
}
