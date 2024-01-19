using AdvertisingSystem.Identity.Application.Interfaces;
using AdvertisingSystem.Identity.Application.UseCases.Commands;
using FluentValidation;

namespace AdvertisingSystem.Identity.Application.Validations;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserService _userService;

    public CreateUserCommandValidator(IUserService userService)
    {
        _userService = userService;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.CellNumber)
            .NotEmpty().WithMessage("Cell number is required.");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.")
            .MustAsync(BeUniqueEmail).WithMessage("Email must be unique.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userService.FindByUserNameAsync(email);
        return existingUser == null;
    }
}