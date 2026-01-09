    using AutoMapper;
using PortalCore.Application.Dtos;
using PortalCore.Application.Notes.Commands.CreateNoteAdmin;
using PortalCore.Application.Notes.Commands.UpdateNoteAdmin;
using PortalCore.Domain.Entities;
using System;
using PortalCore.Application.Notes.Commands.CreateNote;
using PortalCore.Application.Notes.Commands.UpdateNote;

namespace PortalCore.Application.Mapping
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<NoteFile, NoteFileDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.FileName, m => m.MapFrom(s => s.FileName))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Note, NoteDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.UserFullName, m => m.MapFrom(s => s.User.UserName))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Content, m => m.MapFrom(s => s.Content))
                .ForMember(d => d.NoteFileDtos, m => m.MapFrom(s => s.NoteFiles))
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserId))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateNoteAdminCommand, Note>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Content, m => m.MapFrom(s => s.Content))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UpdateNoteAdminCommand, Note>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Content, m => m.MapFrom(s => s.Content))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateNoteCommand, Note>()
                .ForMember(d => d.Id, m => m.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Content, m => m.MapFrom(s => s.Content))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UpdateNoteCommand, Note>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title))
                .ForMember(d => d.Content, m => m.MapFrom(s => s.Content))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}