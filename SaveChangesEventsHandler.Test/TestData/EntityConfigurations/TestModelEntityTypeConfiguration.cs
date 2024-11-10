using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveChangesEventsHandler.Test.TestData.Entites;
using SaveChangesEventsHandler.Test.TestData.EntityConfigurations.Abstraction;

namespace SaveChangesEventsHandler.Test.TestData.EntityConfigurations
{

    public class TestModelEntityTypeConfiguration : BaseEntityConfiguration<TestModel>
    {
        public override void Configure(EntityTypeBuilder<TestModel> builder)
        {
            builder.HasMany(e => e.TestModelNavigations)
                .WithOne(t => t.TestModel)
                .HasForeignKey(t=>t.TestModelId);

            builder.Navigation(e => e.TestModelNavigations).AutoInclude();

            base.Configure(builder);
        }
    }
}
