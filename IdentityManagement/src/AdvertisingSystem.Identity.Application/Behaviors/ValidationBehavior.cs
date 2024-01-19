using AdvertisingSystem.Identity.Application.Validations;
using AdvertisingSystem.Identity.Shared.Exceptions;
using FluentValidation;
using MediatR;

namespace AdvertisingSystem.Identity.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)

    {
        var context = new ValidationContext<TRequest>(request);
        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)
            ));

        var validationErrors = validationFailures.Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToList();

        if (validationErrors.Any())
            throw new BusinessValidationException(validationErrors);

        var resp = await next();

        return resp;
    }
}