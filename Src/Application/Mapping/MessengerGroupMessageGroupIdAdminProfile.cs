using AutoMapper;
using DNTPersianUtils.Core;
using PortalCore.Application.Dtos;
using PortalCore.Domain.Entities;
using System;

namespace PortalCore.Application.Mapping
{
    public class MessengerGroupMessageGroupIdAdminProfile : Profile
    {
        public MessengerGroupMessageGroupIdAdminProfile()
        {
            CreateMap<MessengerGroupMessage, MessengerDisplayGroupMessagesByGroupIdAdminDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.MessageBody, m => m.MapFrom(s => s.MessageBody))
                .ForMember(d => d.SendDate, m => m.MapFrom(s => s.SendDateTime.Date.ToShortPersianDateTimeString(true)))
                .ForMember(d => d.ParentId, m => m.MapFrom(s => s.ParentId))
                .ForMember(d => d.ParentMessage, m => m.MapFrom(s => s.ParentMessage == null ? string.Empty : s.ParentMessage.MessageBody))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
