namespace SmplBank.Domain.Common
{
    /// <summary>
    /// https://jasonwatmore.com/post/2020/07/16/aspnet-core-3-hash-and-verify-passwords-with-bcrypt
    /// </summary>
    public interface ISecurityService
    {
        string HashPassword(string password);
        bool ValidatePassword(string password, string hashedPassword);
    }
}
