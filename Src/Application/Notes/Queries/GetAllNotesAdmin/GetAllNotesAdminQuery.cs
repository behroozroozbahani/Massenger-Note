using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PortalCore.Application.Notes.Queries.GetAllNotesAdmin
{
    public class GetAllNotesAdminQuery : IRequest<IEnumerable<NoteDto>>
    {

    }

    public class GetAllNotesAdminQueryHandler : IRequestHandler<GetAllNotesAdminQuery, IEnumerable<NoteDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllNotesAdminQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<NoteDto>> Handle(GetAllNotesAdminQuery request, CancellationToken cancellationToken)
        {
            var notes = await _applicationDbContext.Notes
                .Include(n => n.User)
                .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var rol = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (rol != true)
            {
                throw new UnauthorizedAccessException();
            }

            return notes;
            
        }
    }
}