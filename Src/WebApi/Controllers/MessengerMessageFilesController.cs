using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.MessengerMessageFiles.Queries.GetMessengerPrivateMessageByOriginalFileId;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using PortalCore.Application.MessengerMessageFiles.Queries.GetMessengerGroupMessageByNailFileId;
using PortalCore.Application.MessengerMessageFiles.Queries.GetMessengerGroupMessageByOriginalFileId;
using PortalCore.Application.MessengerMessageFiles.Queries.GetMessengerPrivateMessageByNailFileId;

namespace PortalCore.WebApi.Controllers
{
    public class MessengerMessageFilesController : ApiControllerBase
    {
        /// <summary>
        /// نمایش فایل اصلی پیام خصوصی با آی دی
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetOriginalPrivateMessageFileById")]
        public async Task<FileStreamResult> GetOriginalPrivateMessageFileById(Guid id)
        {
            var result = await Mediator.Send(new GetMessengerPrivateMessageByOriginalFileIdQuery() { Id = id });

            if (result.FileContents is null)
                throw new NotFoundException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }

        /// <summary>
        /// نمایش فایل تغییر یافته پیام خصوصی با آی دی
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetNailPrivateMessageFileById")]
        public async Task<FileStreamResult> GetNailPrivateMessageFileById(Guid id)
        {
            var result = await Mediator.Send(new GetMessengerPrivateMessageByNailFileIdQuery() { Id = id });

            if (result.FileContents is null)
                throw new NotFoundException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }

        /// <summary>
        /// نمایش فایل اصلی پیام گروه با آی دی
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetOriginalGroupMessageFileById")]
        public async Task<FileStreamResult> GetOriginalGroupMessageFileById(Guid id)
        {
            var result = await Mediator.Send(new GetMessengerGroupMessageByOriginalFileIdQuery() { Id = id });

            if (result.FileContents is null)
                throw new NotFoundException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }

        /// <summary>
        /// نمایش فایل تغییر یافته پیام گروه با آی دی
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetNailGroupMessageFileById")]
        public async Task<FileStreamResult> GetNailGroupMessageFileById(Guid id)
        {
            var result = await Mediator.Send(new GetMessengerGroupMessageByNailFileIdQuery() { Id = id });

            if (result.FileContents is null)
                throw new NotFoundException();

            return File(result.FileContents, result.ContentType, result.FileDownloadName);
        }
    }
}
