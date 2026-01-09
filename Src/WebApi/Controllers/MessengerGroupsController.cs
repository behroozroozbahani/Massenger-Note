using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.MessengerGroups.Commands.CreateMessengerGroup;
using PortalCore.Application.MessengerGroups.Commands.CreateMessengerGroupNewMember;
using PortalCore.Application.MessengerGroups.Commands.DeleteMessengerGroupById;
using PortalCore.Application.MessengerGroups.Commands.UpdateMessengerGroup;
using PortalCore.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using PortalCore.Application.MessengerGroups.Commands.DeleteMessengerGroupMemberById;
using PortalCore.Application.MessengerGroups.Commands.DeleteMessengerGroupMembers;

namespace PortalCore.WebApi.Controllers
{
    public class MessengerGroupsController : ApiControllerBase
    {
        /// <summary>
        /// ایجاد یک گروه برای کاربر لاگین کرده
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("CreateGroup")]
        [Consumes("multipart/form-data")]
        public async Task<OkApiResult<bool>> CreateGroup([FromForm] CreateMessengerGroupCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// اضافه کردن یک یا چند کاربر جدید برای کاربر لاگین کرده
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("CreateGroupNewMember")]
        [Consumes("multipart/form-data")]
        public async Task<OkApiResult<bool>> CreateGroupNewMember([FromForm] CreateMessengerGroupNewMemberCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// ویرایش گروه برای ایجاد کننده گروه
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("UpdateGroup")]
        public async Task<OkApiResult<bool>> UpdateGroup([FromForm] UpdateMessengerGroupCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف گروه با آی دی برای ایجاد کننده گروه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteGroupById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteGroupById([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteMessengerGroupByIdCommand() { GroupId = id });
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف یک عضو با آی دی برای کاربر لاگین کرده
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteGroupMemberById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteGroupMemberById([FromBody] DeleteMessengerGroupMemberByIdCommand command)
        {
            if (command.UserId == Guid.Empty || command.GroupId==Guid.Empty)
                return BadRequest();

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// حذف چند عضو با آی دی برای کاربر لاگین کردهد
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteGroupMembers")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteGroupMembers([FromBody] DeleteMessengerGroupMembersCommand command)
        {
            if (!command.UserIds.Any())
                return BadRequest();

            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }
    }
}