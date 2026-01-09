using System;
using System.Linq;
using System.Threading.Tasks;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Logger;
using PortalCore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace PortalCore.Persistence.Services.Logger
{
    public class AppLogItemsService : IAppLogItemsService
    {
        private readonly DbSet<AppLogItem> _appLogItems;
        private readonly IApplicationDbContext _unitOfWork;

        public AppLogItemsService(IApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _appLogItems = _unitOfWork.Set<AppLogItem>();
        }

        public Task DeleteAllAsync(string logLevel = "")
        {
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                _appLogItems.RemoveRange(_appLogItems);
            }
            else
            {
                var query = _appLogItems.Where(l => l.LogLevel == logLevel);
                _appLogItems.RemoveRange(query);
            }

            return _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid logItemId)
        {
            var itemToRemove = await _appLogItems.FirstOrDefaultAsync(x => x.Id.Equals(logItemId));
            if (itemToRemove != null)
            {
                _appLogItems.Remove(itemToRemove);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public Task DeleteOlderThanAsync(DateTimeOffset cutoffDateUtc, string logLevel = "")
        {
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                var query = _appLogItems.Where(l => l.CreatedDateTime < cutoffDateUtc);
                _appLogItems.RemoveRange(query);
            }
            else
            {
                var query = _appLogItems.Where(l => l.CreatedDateTime < cutoffDateUtc && l.LogLevel == logLevel);
                _appLogItems.RemoveRange(query);
            }

            return _unitOfWork.SaveChangesAsync();
        }

        public Task<int> GetCountAsync(string logLevel = "")
        {
            return string.IsNullOrWhiteSpace(logLevel) ?
                            _appLogItems.CountAsync() :
                            _appLogItems.Where(l => l.LogLevel == logLevel).CountAsync();
        }

        //public async Task<PagedAppLogItemsViewModel> GetPagedAppLogItemsAsync(
        //    int pageNumber,
        //    int pageSize,
        //    SortOrder sortOrder,
        //    string logLevel = "")
        //{
        //    var offset = (pageSize * pageNumber) - pageSize;

        //    var query = string.IsNullOrWhiteSpace(logLevel) ?
        //                     _appLogItems :
        //                     _appLogItems.Where(l => l.LogLevel == logLevel);

        //    query = sortOrder == SortOrder.Descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);

        //    return new PagedAppLogItemsViewModel
        //    {
        //        Paging =
        //        {
        //            TotalItems = await query.CountAsync()
        //        },
        //        AppLogItems = await query.Skip(offset).Take(pageSize).ToListAsync()
        //    };
        //}
    }
}
