using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveChangesEventsHandler.Test.TestData.Entites;
using SaveChangesEventsHandler.Test.TestData.EntityConfigurations.Abstraction;

namespace SaveChangesEventsHandler.Test.TestData.EntityConfigurations
{

    public class TestModelNavigationEntityTypeConfiguration : BaseEntityConfiguration<TestModelNavigation>
    {
        public override void Configure(EntityTypeBuilder<TestModelNavigation> builder)
        {
            base.Configure(builder);
        }
    }
}
