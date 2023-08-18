namespace AdvertisingSystem.Identity.Shared.Exceptions;

public class BusinessException : Exception
{
    protected BusinessException(string message) : base(message)
    {
        Errors = new List<string>();
    }

    public List<string> Errors { get; set; }
}