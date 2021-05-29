using SmplBank.Domain.Common;

namespace SmplBank.Infrastructure.Common
{
    public class SecurityService : ISecurityService
    {
        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool ValidatePassword(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
