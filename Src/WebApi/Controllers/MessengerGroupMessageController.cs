using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.Dtos;
using PortalCore.Application.MessengerGroupMessages.Commands.CreateMessengerGroupMessage;
using PortalCore.Application.MessengerGroupMessages.Commands.DeleteMessengerGroupMessageById;
using PortalCore.Application.MessengerGroupMessages.Commands.DeleteMessengerGroupMessages;
using PortalCore.Application.MessengerGroupMessages.Queries.GetAllMessengerGroupMessages;
using PortalCore.Application.MessengerGroupMessages.Queries.GetAllMessengerGroupMessagesAdmin;
using PortalCore.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using PortalCore.Application.MessengerGroupMessages.Queries.GetMessengerGroupMessagesByGroupId;
using PortalCore.Application.MessengerGroupMessages.Queries.GetMessengerGroupMessagesByGroupIdAdmin;

namespace PortalCore.WebApi.Controllers
{
    public class MessengerGroupMessageController : ApiControllerBase
    {
        /// <summary>
        /// نمایش همه پیام های گروه ها برای ادمین
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllGroupMessagesProtected")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayAllGroupMessagesAdminDto>>> GetAllGroupMessagesProtected()
        {
            return new OkApiResult<IEnumerable<MessengerDisplayAllGroupMessagesAdminDto>>(
                await Mediator.Send(new GetAllMessengerGroupMessagesAdminQuery()));
        }

        /// <summary>
        /// نمایش پیام های گروه خاصی با آی دی برای ادمین
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetGroupMessagesByGroupIdProtected")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>>> GetGroupMessagesByGroupIdProtected(Guid id)
        {
            return new OkApiResult<IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>>(
                await Mediator.Send(new GetMessengerGroupMessagesByGroupIdAdminQuery() { GroupId = id }));
        }

        /// <summary>
        /// نمایش تمامی گروه های عضو برای کاربر لاگین شده
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllGroupMessages")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayAllGroupMessagesDto>>> GetAllGroupMessages()
        {
            return new OkApiResult<IEnumerable<MessengerDisplayAllGroupMessagesDto>>(
                await Mediator.Send(new GetAllMessengerGroupMessagesQuery()));
        }

        /// <summary>
        /// نمایش تمامی پیام های گروه خاص برای کاربر لاگین شده
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetGroupMessagesByGroupId")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>>> GetGroupMessagesByGroupId(Guid id)
        {
            return new OkApiResult<IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>>(
                await Mediator.Send(new GetMessengerGroupMessagesByGroupIdQuery() { GroupId = id }));
        }

        /// <summary>
        /// ایجاد پیام برای کاربر لاگین کرده
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("CreateGroupMessage")]
        public async Task<OkApiResult<bool>> CreateGroupMessage([FromForm] CreateMessengerGroupMessageCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف یک  پیام گروه با آی دی برای کاربر لاگین کرده
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteGroupMessageById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteGroupMessageById([FromBody] DeleteMessengerGroupMessageByIdCommand command)
        {
            if (command.MessageId == Guid.Empty)
                return BadRequest();

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف پیام های گروه برای کاربر لاگین کرده 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteGroupMessages")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteGroupMessages([FromBody] DeleteMessengerGroupMessagesCommand command)
        {
            if (!command.MessageIds.Any())
                return BadRequest();

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }
    }
}