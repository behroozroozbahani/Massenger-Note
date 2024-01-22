using AutoMapper;
using PortalCore.Application.Dtos;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Application.Mapping
{
    public class MessengerAllUsersProfile : Profile
    {
        public MessengerAllUsersProfile()
        {
            CreateMap<User, MessengerDisplayAllUsersDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, m => m.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, m => m.MapFrom(s => s.LastName))
                .ForMember(d => d.ProfileImage, m => m.MapFrom(s => s.ProfileImage))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
