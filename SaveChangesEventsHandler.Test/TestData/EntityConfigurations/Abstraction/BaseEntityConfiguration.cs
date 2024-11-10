﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveChangesEventHandlers.Core.Abstraction.Entities;

namespace SaveChangesEventsHandler.Test.TestData.EntityConfigurations.Abstraction
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
