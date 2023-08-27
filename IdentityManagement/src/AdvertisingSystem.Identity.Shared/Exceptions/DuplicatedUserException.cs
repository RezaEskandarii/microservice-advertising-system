namespace AdvertisingSystem.Identity.Shared.Exceptions;

public class DuplicatedUserException : BusinessException
{
    public DuplicatedUserException(string username) : base(username)
    {
    }

    public override string Message => $"Duplicated user with unique identifier: {base.Message}";
}