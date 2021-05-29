namespace SmplBank.Infrastructure.Exception
{
    public class ConcurrentException : System.Exception
    {
        public ConcurrentException(string entityName) : base($"Concurrent exception happens on entity {entityName}")
        {

        }
    }
}
