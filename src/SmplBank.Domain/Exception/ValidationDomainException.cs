namespace SmplBank.Domain.Exception
{
    public class ValidationDomainException : DomainException
    {
        public ValidationDomainException(string message = "") : base(DomainExceptionType.Validation, message)
        {
        }
    }
}
