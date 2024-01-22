using FluentValidation;

namespace PortalCore.Application.Account.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
          .NotNull()
          .WithName("RefreshToken")
          .WithMessage("لطفا {PropertyName} را وارد کنید")
          .NotEmpty()
          .WithMessage("{PropertyName} نمیتواند خالی باشد");
        }
    }
}
