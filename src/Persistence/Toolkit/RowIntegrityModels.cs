using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace PortalCore.Persistence.Toolkit
{
    public class AuditEntry
    {
        public EntityEntry EntityEntry { set; get; } = null!;
        public IList<AuditProperty> AuditProperties { set; get; } = new List<AuditProperty>();

        public AuditEntry() { }

        public AuditEntry(EntityEntry entry)
        {
            EntityEntry = entry;
        }
    }

    public class AuditProperty
    {
        public string Name { set; get; } = null!;
        public object? Value { set; get; }

        public bool IsTemporary { set; get; }
        public PropertyEntry PropertyEntry { set; get; } = null!;

        public AuditProperty() { }

        public AuditProperty(string name, object? value, bool isTemporary, PropertyEntry property)
        {
            Name = name;
            Value = value;
            IsTemporary = isTemporary;
            PropertyEntry = property;
        }
    }
}
