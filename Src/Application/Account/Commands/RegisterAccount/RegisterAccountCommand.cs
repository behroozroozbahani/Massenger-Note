using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Account.Commands.RegisterAccount
{
    public class RegisterAccountCommand : IRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalCode { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

    public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public RegisterAccountCommandHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {

            var entity = _mapper.Map<User>(request);
            var result = await _userManager.CreateAsync(entity, request.Password);
            if (!result.Succeeded)
                throw new Exception(JsonConvert.SerializeObject(result.Errors).ToString());

            var roleResut = await _userManager.AddToRoleAsync(entity, "User");

            return Unit.Value;
        }
    }
}
