using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalCore.Application.Dtos;
using PortalCore.WebApi.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using PortalCore.Application.Users.Queries.GetAllUsers;

namespace PortalCore.WebApi.Controllers
{
    public class UsersController : ApiControllerBase
    {
        /// <summary>
        /// GetAllUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        [DisplayName("GetAllUsers")]
        public async Task<OkApiResult<IEnumerable<MessengerDisplayAllUsersDto>>> GetAllUsers()
        {
            return new OkApiResult<IEnumerable<MessengerDisplayAllUsersDto>>(
                await Mediator.Send(new GetAllUsersQuery()));
        }
    }
}
