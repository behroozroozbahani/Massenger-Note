using AutoMapper;
using PortalCore.Application.Account.Commands.RegisterAccount;
using PortalCore.Domain.Entities.Identity;
using System;

namespace PortalCore.Application.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterAccountCommand, User>()
                .ForMember(s => s.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(s => s.FirstName, m => m.MapFrom(s => s.FirstName))
                .ForMember(s => s.LastName, m => m.MapFrom(s => s.LastName))
                .ForMember(s => s.PhoneNumber, m => m.MapFrom(s => s.PhoneNumber))
                .ForMember(s => s.UserName, m => m.MapFrom(s => s.PhoneNumber))
                .ForMember(s => s.SerialNumber, m => m.MapFrom(_ => string.Empty))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
