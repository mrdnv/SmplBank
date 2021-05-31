using System;

namespace SmplBank.Domain.Dto.AccountDto
{
    public record AccountDto
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}
