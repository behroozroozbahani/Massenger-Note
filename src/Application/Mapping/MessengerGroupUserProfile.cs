using AutoMapper;
using PortalCore.Application.MessengerGroups.Commands.CreateMessengerGroupNewMember;
using PortalCore.Domain.Entities;
using System;

namespace PortalCore.Application.Mapping
{
    public class MessengerGroupUserProfile : Profile
    {
        public MessengerGroupUserProfile()
        {
            CreateMap<CreateMessengerGroupNewMemberCommand, MessengerGroupUser>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserIds))
                .ForMember(d => d.MessengerGroupId, m => m.MapFrom(s => s.GroupId))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
