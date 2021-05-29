namespace SmplBank.Domain.Entity
{
    public class Account : Entity
    {
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
