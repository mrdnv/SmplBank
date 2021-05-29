namespace SmplBank.Domain.Exception
{
    public class EntityNotFoundDomainException : DomainException
    {
        public EntityNotFoundDomainException(string message = "") : base(DomainExceptionType.NotFound, message)
        {
        }
    }
}
