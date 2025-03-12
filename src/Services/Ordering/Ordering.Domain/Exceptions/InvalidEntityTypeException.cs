namespace Ordering.Domain.Exceptions
{
    public class InvalidEntityTypeException : ApplicationException
    {
        public InvalidEntityTypeException(string entity, object type) 
            : base($"Entity \"{entity}\" not supported type: {type}") 
        { 
        }
    }
}
