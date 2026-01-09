using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalCore.Application.Account.Commands.LogoutAccount
{
    public class LogoutAccountCommandValidator : AbstractValidator<LogoutAccountCommand>
    {
        public LogoutAccountCommandValidator()
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
