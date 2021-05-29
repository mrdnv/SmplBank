namespace SmplBank.Domain.Exception
{
    public class InternalErrorDomainException : DomainException
    {
        public InternalErrorDomainException(string message) 
            : base(DomainExceptionType.InternalError, message)
        {
        }
    }
}
