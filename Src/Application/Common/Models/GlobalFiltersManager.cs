using PortalCore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PortalCore.Application.Common.Models
{
    public static class GlobalFiltersManager
    {
        public static void ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(eType => typeof(ISoftDeleteEntity).IsAssignableFrom(eType.ClrType)))
            {
                entityType.addSoftDeleteQueryFilter();
            }
        }

        private static void addSoftDeleteQueryFilter(this IMutableEntityType entityData)
        {
            var methodToCall = typeof(GlobalFiltersManager)
                .GetMethod(nameof(getSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(entityData.ClrType);
            if (methodToCall is not null)
            {
                var filter = methodToCall.Invoke(null, new object[] { });
                if (filter is not null)
                {
                    entityData.SetQueryFilter((LambdaExpression)filter);
                }
            }
        }

        private static LambdaExpression getSoftDeleteFilter<TEntity>() where TEntity : ISoftDeleteEntity
        {
            //way ShadowProperties
            Expression<Func<TEntity, bool>> filter =
               e => EF.Property<bool>(e, AuditableShadowProperties.IsDeleted) == false;
            return filter;

            //way abstract class instead interface
            //return (Expression<Func<TEntity, bool>>)(entity => !entity.IsDeleted);
        }
    }
}
