using FamilyBudget.Server.Infractructure.Configuration;
using FamilyBudget.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FamilyBudget.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        private readonly DbContextConfiguration _contextOptions;

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<DbContextConfiguration> contextOptions,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            _contextOptions = contextOptions.Value;
        }

        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetEntry> BudgetEntries { get; set; }
        public DbSet<BudgetEntryCategory> BudgetEntryCategories { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (!_contextOptions.TurnOffUpdatingTimestamps)
            {
                UpdateTimestamps();
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (!_contextOptions.TurnOffUpdatingTimestamps)
            {
                UpdateTimestamps();
            }

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void UpdateTimestamps()
        {
            DateTime dateNow = DateTime.Now;

            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                object entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    PropertyInfo createdAtProperty = entity.GetType().GetProperty(nameof(BaseEntity.CreatedAt));

                    if (createdAtProperty is not null)
                    {
                        createdAtProperty.SetValue(entity, dateNow, null);
                    }
                }

                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    PropertyInfo updatedAtProperty = entity.GetType().GetProperty(nameof(BaseEntity.UpdatedAt));

                    if (updatedAtProperty is not null)
                    {
                        updatedAtProperty.SetValue(entity, dateNow, null);
                    }
                }
            }
        }
    }
}