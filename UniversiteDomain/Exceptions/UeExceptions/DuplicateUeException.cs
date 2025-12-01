namespace UniversiteDomain.Exceptions.UeExceptions;

public class DuplicateUeException : Exception
{
    public DuplicateUeException(string message) : base(message)
    {
    }
}