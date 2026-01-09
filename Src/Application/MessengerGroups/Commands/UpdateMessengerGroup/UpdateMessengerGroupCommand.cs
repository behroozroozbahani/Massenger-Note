using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerGroups.Commands.UpdateMessengerGroup
{
    public class UpdateMessengerGroupCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public IFormFile? Image { get; set; }
    }

    public class UpdateGroupUserCommandHandler : IRequestHandler<UpdateMessengerGroupCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateGroupUserCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(UpdateMessengerGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.MessengerGroups
                .Include(x => x.MessengerGroupUsers)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();
            
            //User logged in
            if (entity.OwnerId != _httpContextAccessor.HttpContext.GetUserId())
            {
                throw new UnauthorizedAccessException();
            }

            _mapper.Map(request, entity);

            if (request.Image != null)
            {
                if (!string.IsNullOrEmpty(entity.Image))
                {
                    _customUploadFileService.DeleteFile("MessengerGroups", entity.Image);
                }

                var saveFileResult = await _customUploadFileService.UploadFileAsync("MessengerGroups", request.Image);
                entity.Image = saveFileResult.FileName;
            }

            _applicationDbContext.MessengerGroups.Update(entity);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
