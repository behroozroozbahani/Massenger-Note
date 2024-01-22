using AutoMapper;
using PortalCore.Application.Dtos;
using PortalCore.Domain.Entities;
using System;
using PortalCore.Application.MessengerPrivateMessages.Commands.CreateMessengerPrivateMessage;

namespace PortalCore.Application.Mapping
{
    public class MessengerPrivateMessageProfile : Profile
    {
        public MessengerPrivateMessageProfile()
        {
            CreateMap<MessengerMessageFile, MessengerMessageFileDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.FileName, m => m.MapFrom(s => s.FileName))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateMessengerPrivateMessageCommand, MessengerPrivateMessage>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.MessageBody, m => m.MapFrom(s => s.MessageBody))
                .ForMember(d => d.ParentId, m => m.MapFrom(s => s.ParentId))
                .ForMember(d => d.RecipientId, m => m.MapFrom(s => s.RecipientId))
                .ForMember(d => d.SendDateTime, m => m.MapFrom(_ => DateTimeOffset.Now))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
