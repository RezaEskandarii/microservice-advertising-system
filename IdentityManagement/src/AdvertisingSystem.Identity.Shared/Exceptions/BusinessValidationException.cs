using AdvertisingSystem.Identity.Application.Validations;

namespace AdvertisingSystem.Identity.Shared.Exceptions;

public class BusinessValidationException : BusinessException
{
    public BusinessValidationException(ICollection<string> errors) : base(errors)
    {
    }

    public BusinessValidationException(ICollection<ValidationError> errors)
    {
    }
}