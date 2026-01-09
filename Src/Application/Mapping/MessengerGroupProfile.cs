using AutoMapper;
using PortalCore.Application.Dtos;
using PortalCore.Application.MessengerGroups.Commands.CreateMessengerGroup;
using PortalCore.Application.MessengerGroups.Commands.UpdateMessengerGroup;
using PortalCore.Domain.Entities;
using System;

namespace PortalCore.Application.Mapping
{
    public class MessengerGroupProfile : Profile
    {
        public MessengerGroupProfile()
        {
            CreateMap<MessengerGroup, MessengerGroupDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateMessengerGroupCommand, MessengerGroup>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UpdateMessengerGroupCommand, MessengerGroup>()
                .ForMember(d => d.Id, m => m.MapFrom(x => x.Id))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
