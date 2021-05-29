namespace SmplBank.Domain.Exception
{
    public abstract class DomainException : System.Exception
    {
        protected DomainException(DomainExceptionType domainExceptionType, string message) : base(message)
        {
            DomainExceptionType = domainExceptionType;
        }

        public DomainExceptionType DomainExceptionType { get; }
    }
}
