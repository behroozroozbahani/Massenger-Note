using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.Dtos;
using PortalCore.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using PortalCore.Application.MessengerPrivateMessages.Commands.CreateMessengerPrivateMessage;
using PortalCore.Application.MessengerPrivateMessages.Commands.DeleteMessengerPrivateMessageAdminById;
using PortalCore.Application.MessengerPrivateMessages.Commands.DeleteMessengerPrivateMessageById;
using PortalCore.Application.MessengerPrivateMessages.Queries.GetAllMessengerPrivateMessages;
using PortalCore.Application.MessengerPrivateMessages.Queries.GetAllMessengerPrivateMessagesAdmin;
using PortalCore.Application.MessengerPrivateMessages.Queries.GetMessengerPrivateMessageAdminBySenderRecipientId;
using PortalCore.Application.MessengerPrivateMessages.Queries.GetMessengerPrivateMessageBySenderRecipientId;

namespace PortalCore.WebApi.Controllers
{
    public class MessengerPrivateMessageController : ApiControllerBase
    {
        /// <summary>
        /// نمایش همه پیامهای شخصی برای ادمین
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllPrivateMessagesProtected")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayAllPrivateMessagesAdminDto>>> GetAllPrivateMessagesProtected()
        {
            return new OkApiResult<IEnumerable<MessengerDisplayAllPrivateMessagesAdminDto>>(
                await Mediator.Send(new GetAllMessengerPrivateMessagesAdminQuery()));
        }

        /// <summary>
        /// نمایش یک پیام با آی دی برای ادمین
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetPrivateMessageProtectedBySenderRecipientId")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>>> GetPrivateMessageProtectedBySenderRecipientId(Guid id)
        {
            return new OkApiResult<IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>>(
                await Mediator.Send(new GetMessengerPrivateMessageAdminBySenderRecipientIdQuery() { SenderRecipientId = id }));
        }

        /// <summary>
        /// نمایش همه پیامهای شخصی برای کاربر لاگین کرده
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllPrivateMessages")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayAllPrivateMessagesDto>>> GetAllPrivateMessages()
        {
            return new OkApiResult<IEnumerable<MessengerDisplayAllPrivateMessagesDto>>(
                await Mediator.Send(new GetAllMessengerPrivateMessagesQuery()));
        }

        /// <summary>
        /// نمایش یک پیام برای کاربر لاگین کرده
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetPrivateMessageBySenderRecipientId")]
        public async Task<OkApiResult<MessengerDisplayMessagesDto>> GetPrivateMessageBySenderRecipientId(Guid id)
        {
            return new OkApiResult<MessengerDisplayMessagesDto>(
                await Mediator.Send(new GetMessengerPrivateMessageBySenderRecipientIdQuery() { SenderRecipientId = id }));
        }

        /// <summary>
        /// ایجاد یک پیام شخصی برای کاربر لاگین کرده
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("CreatePrivateMessage")]
        public async Task<OkApiResult<bool>> CreatePrivateMessage([FromForm] CreateMessengerPrivateMessageCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف یک پیام شخصی با آی دی برای کاربر لاگین کرده
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeletePrivateMessageById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeletePrivateMessageById([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteMessengerPrivateMessageByIdCommand() { Id = id });
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف یک پیام شخصی با آی دی برای ادمین
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeletePrivateMessageProtectedById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeletePrivateMessageProtectedById([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteMessengerPrivateMessageAdminByIdCommand() { Id = id });
            return new OkApiResult<bool>(true);
        }
    }
}