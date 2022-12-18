using DataProvider.EntityFramework;
using FamilyBudget.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyBudget.Server.Data.Configuration
{
    public class BudgetConfiguration : BaseEntityConfiguration<Budget>
    {
        public override void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder
                .HasMany(x => x.UsersAssignedToBudget)
                .WithMany(x => x.UserBudgets)
                .UsingEntity(x => x.ToTable("UserBudgets"));

            builder
                .HasMany(x => x.BudgetEntries)
                .WithOne(x => x.Budget);

            base.Configure(builder);
        }
    }
}
