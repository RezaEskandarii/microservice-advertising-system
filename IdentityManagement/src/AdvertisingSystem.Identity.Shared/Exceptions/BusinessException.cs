namespace AdvertisingSystem.Identity.Shared.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
        Errors = new List<string>();
    }

    public BusinessException()
    {
    }

    public BusinessException(ICollection<string> errors)
    {
        Errors.AddRange(errors);
    }

    public List<string> Errors { get; set; } = new List<string>();
}