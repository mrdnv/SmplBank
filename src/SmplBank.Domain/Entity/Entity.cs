using System;

namespace SmplBank.Domain.Entity
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public byte[] RowVersion { get; set; }
    }
}
