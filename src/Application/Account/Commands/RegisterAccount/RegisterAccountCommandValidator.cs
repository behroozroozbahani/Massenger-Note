using FluentValidation;
using PortalCore.Application.Common.Interfaces;

namespace PortalCore.Application.Account.Commands.RegisterAccount
{
    public class RegisterAccountCommandValidator : AbstractValidator<RegisterAccountCommand>
    {
        private readonly IAccountService _accountService;
        public RegisterAccountCommandValidator(IAccountService accountService)
        {
            _accountService = accountService;

            RuleFor(x => x.FirstName)
          .NotNull()
          .WithName("نام")
          .WithMessage("لطفا {PropertyName} را وارد کنید")
          .NotEmpty()
          .WithMessage("{PropertyName} نمیتواند خالی باشد");

            RuleFor(x => x.LastName)
          .NotNull()
          .WithName("نام خانوادگی")
          .WithMessage("لطفا {PropertyName} را وارد کنید")
          .NotEmpty()
          .WithMessage("{PropertyName} نمیتواند خالی باشد");

            RuleFor(x => x.PhoneNumber)
          .NotNull()
          .WithName("شماره موبایل")
          .WithMessage("لطفا {PropertyName} را وارد کنید")
          .NotEmpty()
          .WithMessage("{PropertyName} نمیتواند خالی باشد")
          .MinimumLength(11)
          .WithMessage("{PropertyName} را به صورت کامل وارد نمایید")
          .MaximumLength(11)
          .WithMessage("{PropertyName} را به صورت کامل وارد نمایید")
           .MustAsync(async (command, s, cancellationToken) => !await _accountService.CheckExistPhoneNumber(command.PhoneNumber, cancellationToken))
                .WithMessage("این {PropertyName} قبلا استفاده شده است")
            .Must((command, s) => _accountService.IsValidPhoneNumber(command.PhoneNumber))
                .WithMessage("{PropertyName} وارد شده نامعتبر است");

            RuleFor(x => x.Password)
          .NotNull()
          .WithName("رمز عبور")
          .WithMessage("لطفا {PropertyName} را وارد کنید")
          .NotEmpty()
          .WithMessage("{PropertyName} نمیتواند خالی باشد");
        }
    }
}
