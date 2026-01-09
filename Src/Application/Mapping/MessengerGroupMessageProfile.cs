using AutoMapper;
using PortalCore.Application.Dtos;
using PortalCore.Domain.Entities;
using System;
using PortalCore.Application.MessengerGroupMessages.Commands.CreateMessengerGroupMessage;

namespace PortalCore.Application.Mapping
{
    public class MessengerGroupMessageProfile : Profile
    {
        public MessengerGroupMessageProfile()
        {
            CreateMap<MessengerMessageFile, MessengerMessageFileDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.FileName, m => m.MapFrom(s => s.FileName))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateMessengerGroupMessageCommand, MessengerGroupMessage>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.MessageBody, m => m.MapFrom(s => s.MessageBody))
                .ForMember(d => d.ParentId, m => m.MapFrom(s => s.ParentId))
                .ForMember(d => d.MessengerGroupId, m => m.MapFrom(s => s.GroupId))
                .ForMember(d => d.SendDateTime, m => m.MapFrom(_ => DateTimeOffset.Now))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
