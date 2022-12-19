using Bogus;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budget;
using NUnit.Framework;

namespace FamilyBudget.Server.Tests.Services.Budgets
{
    [TestFixture]
    public class BudgetServiceTests : ServiceTest
    {
        private readonly BudgetService _sut;

        public BudgetServiceTests()
        {
            _sut = new BudgetService(DbContext, UserProvider);
        }

        [Test]
        public async Task GetUserBudgets_ShouldReturnAllUserBudgetsWithProperties()
        {
            //Arrange
            int budgetsCount = 5;

            var user = await GetMockedUser();

            var budgetAssignedUsers = new List<ApplicationUser> { user };

            var userBudgets = new Faker<Budget>()
                .RuleFor(x => x.Name, f => f.Finance.AccountName())
                .RuleFor(x => x.Balance, f => f.Random.Decimal())
                .RuleFor(x => x.UsersAssignedToBudget, budgetAssignedUsers)
                .Generate(budgetsCount);

            await DbContext.AddRangeAsync(userBudgets);
            await DbContext.SaveChangesAsync();

            //Act
            var result = await _sut.GetUserBudgets();

            //Assert
            Assert.AreEqual(budgetsCount, result.Count);

            foreach (var budget in userBudgets)
            {
                Assert.IsTrue(result.Any(x => x.Id == budget.Id && x.Name == budget.Name && x.Balance == budget.Balance));
            }
        }

        [Test]
        public async Task GetUserBudgets_ShouldNotReturnOtherUsers()
        {
            //Arrange

            var otherUserBudgets = new Faker<Budget>()
                .RuleFor(x => x.Name, f => f.Finance.AccountName())
                .RuleFor(x => x.Balance, f => f.Random.Decimal())
                .Generate(10);

            await DbContext.AddRangeAsync(otherUserBudgets);
            await DbContext.SaveChangesAsync();

            //Act
            var result = await _sut.GetUserBudgets();

            //Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
