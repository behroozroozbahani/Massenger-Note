using FluentValidation;

namespace PortalCore.Application.Account.Commands.LoginAccount
{
    public class LoginAccountCommandValidator : AbstractValidator<LoginAccountCommand>
    {
        public LoginAccountCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithName("نام کاربری")
                .WithMessage("لطفا {PropertyName} را وارد کنید")
                .NotEmpty()
                .WithMessage("{PropertyName} نمیتواند خالی باشد");

            RuleFor(x => x.Password)
                .NotNull()
                .WithName("رمز عبور")
                .WithMessage("لطفا {PropertyName} را وارد کنید")
                .NotEmpty()
                .WithMessage("{PropertyName} نمیتواند خالی باشد");
        }
    }
}
