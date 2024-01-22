using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.Account.Commands.LoginAccount;
using PortalCore.Application.Account.Commands.RefreshToken;
using PortalCore.Application.Account.Commands.RegisterAccount;
using PortalCore.Application.Dtos;
using PortalCore.WebApi.Helpers;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PortalCore.WebApi.Controllers
{
    [AllowAnonymous]
    public class AccountController : ApiControllerBase
    {
        /// <summary>
        /// ثبت نام
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("Register")]
        public async Task<OkApiResult<bool>> Register([FromBody] RegisterAccountCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// ورود
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("Login")]
        public async Task<OkApiResult<Token>> Login([FromBody] LoginAccountCommand command)
        {
            //if (!_validatorService.HasRequestValidCaptchaEntry(Language.Persian, DisplayMode.SumOfTwoNumbers))
            //    throw new BadRequestException("جمع وارد شده نامعتبر است");
            return new OkApiResult<Token>(await Mediator.Send(command));
        }


        /// <summary>
        /// تازه سازی توکن
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("RefreshToken")]
        public async Task<OkApiResult<Token>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            return new OkApiResult<Token>(await Mediator.Send(command));
        }
    }
}
