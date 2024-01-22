using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.Dtos;
using PortalCore.Application.Notes.Commands.CreateNoteAdmin;
using PortalCore.Application.Notes.Commands.DeleteNoteByIdAdmin;
using PortalCore.Application.Notes.Commands.DeleteNoteFileByIdAdmin;
using PortalCore.Application.Notes.Commands.UpdateNoteAdmin;
using PortalCore.Application.Notes.Queries.GetAllNotesAdmin;
using PortalCore.Application.Notes.Queries.GetNoteByIdAdmin;
using PortalCore.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using PortalCore.Application.Notes.Commands.CreateNote;
using PortalCore.Application.Notes.Commands.DeleteNoteById;
using PortalCore.Application.Notes.Commands.DeleteNoteFileById;
using PortalCore.Application.Notes.Commands.UpdateNote;
using PortalCore.Application.Notes.Queries.GetAllNotes;
using PortalCore.Application.Notes.Queries.GetNoteById;

namespace PortalCore.WebApi.Controllers
{
    public class NotesController : ApiControllerBase
    {
        /// <summary>
        /// GetAllNotesProtected
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllNotesProtected")]
        public async Task<OkApiResult<IEnumerable<NoteDto>>> GetAllNotesProtected()
        {
            return new OkApiResult<IEnumerable<NoteDto>>(
                await Mediator.Send(new GetAllNotesAdminQuery()));
        }

        /// <summary>
        /// GetAllNotes
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllNotes")]
        public async Task<OkApiResult<IEnumerable<NoteDto>>> GetAllNotes()
        {
            return new OkApiResult<IEnumerable<NoteDto>>(
                await Mediator.Send(new GetAllNotesQuery()));
        }

        /// <summary>
        /// GetNoteByIdProtected
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetNoteByIdProtected")]
        public async Task<OkApiResult<NoteDto?>> GetNoteByIdProtected(Guid id)
        {
            return new OkApiResult<NoteDto?>(
                await Mediator.Send(new GetNoteByIdAdminQuery() { Id = id }));
        }

        /// <summary>
        /// GetNoteById
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetNoteById")]
        public async Task<OkApiResult<NoteDto?>> GetNoteById(Guid id)
        {
            return new OkApiResult<NoteDto?>(
                await Mediator.Send(new GetNoteByIdQuery() { Id = id }));
        }

        /// <summary>
        /// CreateNoteProtected
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("CreateNoteProtected")]
        [Consumes("multipart/form-data")]
        public async Task<OkApiResult<bool>> CreateNoteProtected([FromForm] CreateNoteAdminCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("CreateNote")]
        [Consumes("multipart/form-data")]
        public async Task<OkApiResult<bool>> CreateNote([FromForm] CreateNoteCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UpdateNoteProtected
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("UpdateNoteProtected")]
        public async Task<OkApiResult<bool>> UpdateNoteProtected([FromForm] UpdateNoteAdminCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// UpdateNote
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("UpdateNote")]
        public async Task<OkApiResult<bool>> UpdateNote([FromForm] UpdateNoteCommand command)
        {
            await Mediator.Send(command);
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// DeleteNoteByIdProtected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteNoteByIdProtected")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteNoteByIdProtected([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteNoteByIdAdminCommand() { Id = id });
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// DeleteNoteById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteNoteById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteNoteById([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteNoteByIdCommand() { Id = id });
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// DeleteNoteFileByIdProtected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteNoteFileByIdProtected")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteNoteFileByIdProtected([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteNoteFileByIdAdminCommand() { Id = id });
            return new OkApiResult<bool>(true);
        }

        /// <summary>
        /// DeleteNoteFileById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("DeleteNoteFileById")]
        public async Task<ActionResult<OkApiResult<bool>>> DeleteNoteFileById([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            await Mediator.Send(new DeleteNoteFileByIdCommand() { Id = id });
            return new OkApiResult<bool>(true);
        }
    }
}