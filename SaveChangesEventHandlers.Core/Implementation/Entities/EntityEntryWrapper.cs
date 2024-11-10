using Microsoft.EntityFrameworkCore.ChangeTracking;
using SaveChangesEventHandlers.Core.Abstraction.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveChangesEventHandlers.Core.Implementation.Entities
{
    public class EntityEntryWrapper
    {
        public object CurrentValues { get; private set; }
        public object OriginalValues { get; private set; }

        public EntityEntryWrapper(EntityEntry entityEntry)
        {
            this.CurrentValues = entityEntry.Entity;
            this.OriginalValues = CreateOriginalObjectWithAllProperties(entityEntry);
        }

        private object CreateOriginalObjectWithAllProperties(EntityEntry entity)
        {
            var originalValues = new Dictionary<string, object?>();

            foreach (var item in entity.Collections)
            {
                if (item.CurrentValue is null)
                {
                    item.CurrentValue = Activator.CreateInstance(item.Metadata.ClrType) as IEnumerable<BaseEntity>;
                }
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            foreach (var item in entity.References)
            {
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            foreach (var item in entity.Properties)
            {
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            var originalObject = Activator.CreateInstance(entity.Metadata.ClrType);

            foreach (var item in originalValues)
            {
                originalObject.GetType().GetProperty(item.Key).SetValue(originalObject, item.Value, null);
            }

            return originalObject;
        }
    }
}
