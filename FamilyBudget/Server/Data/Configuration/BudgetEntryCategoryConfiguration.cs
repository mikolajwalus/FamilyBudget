using DataProvider.EntityFramework;
using FamilyBudget.Server.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyBudget.Server.Data.Configuration
{
    public class BudgetEntryCategoryConfiguration : BaseEntityConfiguration<BudgetEntryCategory>
    {
        public override void Configure(EntityTypeBuilder<BudgetEntryCategory> builder)
        {
            builder
                .HasMany(x => x.BudgetEntries)
                .WithOne(x => x.BudgetEntryCategory);

            base.Configure(builder);
        }
    }
}
