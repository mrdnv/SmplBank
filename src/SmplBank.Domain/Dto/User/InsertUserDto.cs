namespace SmplBank.Domain.Dto.User
{
    public record InsertUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
