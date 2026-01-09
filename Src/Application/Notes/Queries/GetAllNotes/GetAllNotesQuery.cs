using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.Notes.Queries.GetAllNotes
{
    public class GetAllNotesQuery : IRequest<IEnumerable<NoteDto>>
    {

    }

    public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, IEnumerable<NoteDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public GetAllNotesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NoteDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            //if (_httpContextAccessor.HttpContext.User.Identity.GetUserId() == null)
            //{
            //    throw new UnauthorizedAccessException();
            //}

            var notes = await _applicationDbContext.Notes
                .Include(n => n.User)
                .Where(n => n.UserId == Guid.Parse("8ec7224e-0e97-4937-069f-08d9e002a55f"))
                .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return notes;
        }
    }
}