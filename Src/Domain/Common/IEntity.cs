using System;

namespace PortalCore.Domain.Common
{
    public interface IBaseEntity
    {

    }

    public interface IEntity<T> : IBaseEntity
    {
        public T Id { get; set; }
    }

    public interface IEntity : IEntity<Guid>
    {

    }
}