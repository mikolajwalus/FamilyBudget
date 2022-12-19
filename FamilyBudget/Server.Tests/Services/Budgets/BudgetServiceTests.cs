using Bogus;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budget;
using NSubstitute;
using NUnit.Framework;

namespace FamilyBudget.Server.Tests.Services.Budgets
{
    [TestFixture]
    public class BudgetServiceTests : ServiceTest
    {

        [Test]
        public async Task GetUserBudgets_ShouldReturnAllUserBudgetsWithProperties()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                int budgetsCount = 5;

                var user = await GetMockedUser(context);

                var budgetAssignedUsers = new List<ApplicationUser> { user };

                var userBudgets = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, budgetAssignedUsers)
                    .Generate(budgetsCount);

                await context.AddRangeAsync(userBudgets);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act
                var result = await sut.GetUserBudgets();

                //Assert
                Assert.AreEqual(budgetsCount, result.Count);

                foreach (var budget in userBudgets)
                {
                    Assert.IsTrue(result.Any(x => x.Id == budget.Id && x.Name == budget.Name && x.Balance == budget.Balance));
                }
            }
        }

        [Test]
        public async Task GetUserBudgets_ShouldNotReturnOtherUsers()
        {
            using (var context = GetDbContext())
            {
                //Arrange

                var otherUserBudgets = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .Generate(10);

                await context.AddRangeAsync(otherUserBudgets);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act
                var result = await sut.GetUserBudgets();

                //Assert
                Assert.AreEqual(0, result.Count);
            }
        }

        [Test]
        public void GetUserBudgets_ShouldThrowException_IfUserNotExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                UserProvider.UserId.Returns(Guid.NewGuid().ToString());

                var sut = GetSut(context);

                //Act and Assert
                Assert.ThrowsAsync<UserNotExistException>(async () => await sut.GetUserBudgets());
            }
        }

        [Test]
        public async Task GetBudget_ReturnUserBudget_IfExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var user = await GetMockedUser(context);

                var budgetAssignedUsers = new List<ApplicationUser> { user };

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, budgetAssignedUsers)
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act 
                var result = await sut.GetBudget(budget.Id);

                //Assert
                Assert.AreEqual(budget.Id, result.Id);
                Assert.AreEqual(budget.Name, result.Name);
                Assert.AreEqual(budget.Balance, result.Balance);
            }
        }

        [Test]
        public async Task GetBudget_ThrowException_IfExistsButIsNotUsers()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BudgetNotExistException>(async () => await sut.GetBudget(budget.Id));
            }
        }

        [Test]
        public void GetBudget_ThrowException_IfNotExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BudgetNotExistException>(async () => await sut.GetBudget(Guid.NewGuid()));
            }
        }

        private BudgetService GetSut(ApplicationDbContext context) => new BudgetService(context, UserProvider);
    }
}
