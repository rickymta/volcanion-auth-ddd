using FluentValidation;
using Volcanion.Auth.Application.Features.Authentication.Commands;

namespace Volcanion.Auth.Application.Features.Authentication.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailOrPhone)
            .NotEmpty().WithMessage("Email or phone number is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
