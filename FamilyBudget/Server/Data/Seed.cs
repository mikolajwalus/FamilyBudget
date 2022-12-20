using Bogus;
using FamilyBudget.Server.Infractructure.Configuration;
using FamilyBudget.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Data
{
    public static class Seed
    {
        public static async Task SeedData(
            ApplicationDbContext context,
            UserManager<ApplicationUser> signInManager,
            DataConfiguration configuration)
        {
            if (!configuration.SeedData)
            {
                return;
            }

            if (await context.Users.AnyAsync())
            {
                return;
            }

            var users = new Faker<ApplicationUser>()
                .RuleFor(x => x.UserName, f => f.Internet.UserName() + f.IndexGlobal)
                .Generate(50);

            await CreateUsers(signInManager, configuration, users);

            var budgets = new Faker<Budget>()
                .RuleFor(x => x.Name, f => f.Finance.AccountName() + f.IndexGlobal)
                .RuleFor(x => x.UsersAssignedToBudget, f => f.Random.ListItems(users))
                .Generate(50);

            var entriesCategories = new Faker<BudgetEntryCategory>()
                .RuleFor(x => x.Name, f => f.Name.JobArea() + f.IndexGlobal)
                .Generate(50);

            await context.AddRangeAsync(entriesCategories);
            await context.AddRangeAsync(budgets);
            await context.SaveChangesAsync();

            await CreateEntries(context, budgets, entriesCategories);
        }

        private static async Task CreateEntries(ApplicationDbContext context, List<Budget> budgets, List<BudgetEntryCategory> entriesCategories)
        {
            var faker = new Faker();

            var allEntries = new List<BudgetEntry>();

            foreach (var budget in budgets)
            {
                var entries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.BudgetEntryCategoryId, f => f.PickRandom(entriesCategories).Id)
                    .RuleFor(x => x.MoneyAmount, f => f.Finance.Amount())
                    .Generate(faker.Random.Int(0, 50));

                allEntries.AddRange(entries);

                budget.Balance = entries.Sum(x => x.MoneyAmount);
            }

            await context.AddRangeAsync(allEntries);
            await context.SaveChangesAsync();
        }

        private static async Task CreateUsers(UserManager<ApplicationUser> signInManager, DataConfiguration configuration, List<ApplicationUser> users)
        {
            if (configuration.UseOnePassowordForSeededUsers)
            {
                foreach (var user in users)
                {
                    await signInManager.CreateAsync(user, configuration.PasswordForSeededUsers);
                }
            }
            else
            {
                var faker = new Faker<string>()
                    .RuleFor(x => x, f => f.Internet.Password());

                foreach (var user in users)
                {
                    await signInManager.CreateAsync(user, faker.Generate());
                }
            }
        }
    }
}
