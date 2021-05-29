namespace SmplBank.Domain.Exception
{
    public class DuplicationDomainException : DomainException
    {
        public DuplicationDomainException(string message = "") : base(DomainExceptionType.Duplication, message)
        {
        }
    }
}
