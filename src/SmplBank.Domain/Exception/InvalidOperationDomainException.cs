namespace SmplBank.Domain.Exception
{
    public class InvalidOperationDomainException : DomainException
    {
        public InvalidOperationDomainException(string message = "") : base(DomainExceptionType.InvalidOperation, message)
        {
        }
    }
}
