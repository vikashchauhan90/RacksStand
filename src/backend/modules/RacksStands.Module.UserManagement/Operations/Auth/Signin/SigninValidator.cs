using FluentValidation;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

internal class SigninValidator : AbstractValidator<SigninCommand>
{
    public SigninValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.MfaCode).Length(6).When(x => !string.IsNullOrEmpty(x.MfaCode));
    }
}
