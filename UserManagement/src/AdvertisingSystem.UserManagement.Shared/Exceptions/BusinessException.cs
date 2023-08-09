namespace AdvertisingSystem.UserManagement.Shared.Exceptions;

public class BusinessException : Exception
{
    protected BusinessException(string message) : base(message)
    {
    }
}