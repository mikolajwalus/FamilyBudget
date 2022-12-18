using DataProvider.EntityFramework;
using FamilyBudget.Server.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyBudget.Server.Data.Configuration
{
    public class BudgetEntryConfiguration : BaseEntityConfiguration<BudgetEntry>
    {
        public override void Configure(EntityTypeBuilder<BudgetEntry> builder)
        {
            base.Configure(builder);
        }
    }
}
