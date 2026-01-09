using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.Notes.Queries.GetNoteById
{
    public class GetNoteByIdQuery : IRequest<NoteDto>
    {
        public Guid Id { get; set; }
    }

    public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetNoteByIdQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<NoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Notes
                .Include(n => n.NoteFiles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            if (entity.UserId != Guid.Parse("8ec7224e-0e97-4937-069f-08d9e002a55f"))
            {
                throw new UnauthorizedAccessException();
            }

            return _mapper.Map(entity, new NoteDto());
        }
    }
}