using Bogus;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.Budget;
using Microsoft.EntityFrameworkCore;
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
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.GetUserBudgets());
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
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.GetBudget(budget.Id));
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
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.GetBudget(Guid.NewGuid()));
            }
        }

        [Test]
        public async Task CreateBudget_CreatesBudgetPropperly()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var users = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate(5);

                await context.AddRangeAsync(users);
                await context.SaveChangesAsync();

                var userIds = users.Select(x => x.Id).ToList();

                var dto = new BudgetForCreationDto
                {
                    Name = "testName",
                    AssignedUsers = userIds
                };

                var sut = GetSut(context);

                //Act
                var result = await sut.CreateBudget(dto);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .Include(x => x.UsersAssignedToBudget)
                    .FirstOrDefaultAsync(x => x.Id == result.Id);

                var userIdsFromDb = budgetFromDb.UsersAssignedToBudget.Select(x => x.Id).ToList();

                userIds.Add(UserId);

                //Assert
                Assert.AreEqual(dto.Name, result.Name);
                Assert.AreEqual(dto.Name, budgetFromDb.Name);
                Assert.AreEqual(0, result.Balance);
                Assert.AreEqual(0, budgetFromDb.Balance);
                Assert.That(userIdsFromDb, Is.EquivalentTo(userIds));
            }
        }

        [Test]
        public async Task CreateBudget_ThrowException_IfNameAlreadyTaken()
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

                var dto = new BudgetForCreationDto
                {
                    Name = budget.Name,
                };

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.CreateBudget(dto));
            }
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void CreateBudget_ThrowException_IfNameNullEmptyOrWhiteSpace(string name)
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var dto = new BudgetForCreationDto
                {
                    Name = name,
                };

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.CreateBudget(dto));
            }
        }

        [Test]
        public void CreateBudget_ThrowException_IfProvidedUserNotExist()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var dto = new BudgetForCreationDto
                {
                    Name = "Test",
                    AssignedUsers = new List<string> { "aaa" }
                };

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.CreateBudget(dto));
            }
        }

        [Test]
        public async Task UpdateBudget_UpdatesBudgetPropperly()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var users = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate(5);

                var requestingUser = await GetMockedUser(context);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, new List<ApplicationUser> { requestingUser })
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var newName = budget.Name + "1111";

                var dto = new BudgetForUpdateDto
                {
                    Id = budget.Id,
                    Name = newName,
                };

                var sut = GetSut(context);

                //Act
                await sut.UpdateBudget(dto);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == budget.Id);

                //Assert
                Assert.AreEqual(newName, budgetFromDb.Name);
            }
        }

        [Test]
        public async Task UpdateBudget_ThrowException_IfNameAlreadyTaken()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var requestingUser = await GetMockedUser(context);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, new List<ApplicationUser> { requestingUser })
                    .Generate();

                var otherBudget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .Generate();

                await context.AddAsync(budget);
                await context.AddAsync(otherBudget);
                await context.SaveChangesAsync();

                var dto = new BudgetForUpdateDto
                {
                    Id = budget.Id,
                    Name = otherBudget.Name,
                };

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.UpdateBudget(dto));
            }
        }

        [Test]
        public async Task UpdateBudget_ThrowException_IfRequestingUserNotAssignedToBudget()
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

                var dto = new BudgetForUpdateDto
                {
                    Id = budget.Id,
                    Name = budget.Name + "11",
                };

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.UpdateBudget(dto));
            }
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public async Task UpdateBudget_ThrowException_IfNameNullEmptyOrWhiteSpace(string name)
        {
            using (var context = GetDbContext())
            {
                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                //Arrange
                var dto = new BudgetForCreationDto
                {
                    Name = name,
                };

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.CreateBudget(dto));
            }
        }

        [Test]
        public async Task GetUsersAssignedToBudget_ReturnsAssignedUsers()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var users = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate(5);

                await context.AddRangeAsync(users);
                await context.SaveChangesAsync();

                var requestingUser = await GetMockedUser(context);

                users.Add(requestingUser);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, users)
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var userIds = users.Select(x => x.Id).ToList();

                var sut = GetSut(context);

                //Act
                var result = await sut.GetUsersAssignedToBudget(budget.Id);

                var resultUserIds = result.Select(x => x.Id).ToList();

                //Assert
                Assert.That(resultUserIds, Is.EquivalentTo(userIds));
            }
        }

        [Test]
        public async Task GetUsersAssignedToBudget_ThrowException_IfRequestingUserNotAssignedToBudget()
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
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.GetUsersAssignedToBudget(budget.Id));
            }
        }

        [Test]
        public async Task AddUserToBudget_AddUserToBudgetPropperly()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var user = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate();

                var requestingUser = await GetMockedUser(context);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, f => new List<ApplicationUser> { requestingUser })
                    .Generate();

                await context.AddAsync(user);
                await context.AddAsync(budget);

                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act and assert
                await sut.AddUserToBudget(user.Id, budget.Id);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .Include(x => x.UsersAssignedToBudget)
                    .FirstOrDefaultAsync(x => x.Id == budget.Id);

                //Assert
                Assert.That(budgetFromDb.UsersAssignedToBudget.Any(x => x.Id == user.Id));
            }
        }

        [Test]
        public async Task AddUserToBudget_ThrowException_IfRequestingUserNotAssignedToBudget()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .Generate();

                var user = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate();

                await context.AddAsync(user);
                await context.AddAsync(budget);

                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.AddUserToBudget(user.Id, budget.Id));
            }
        }

        [Test]
        public async Task AddUserToBudget_ThrowException_IfUserNotExists()
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
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.AddUserToBudget("TestId", budget.Id));
            }
        }

        [Test]
        public async Task RemoveUserFromBudget_RemoveUserFromBudgetPropperly()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var user = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate();

                await context.AddAsync(user);
                await context.SaveChangesAsync();

                var requestingUser = await GetMockedUser(context);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, f => new List<ApplicationUser> { requestingUser, user })
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act and assert
                await sut.RemoveUserFromBudget(user.Id, budget.Id);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .Include(x => x.UsersAssignedToBudget)
                    .FirstOrDefaultAsync(x => x.Id == budget.Id);

                //Assert
                Assert.That(!budgetFromDb.UsersAssignedToBudget.Any(x => x.Id == user.Id));
            }
        }

        [Test]
        public async Task RemoveUserFromBudget_ThrowException_IfRequestingUserNotAssignedToBudget()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var user = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate();

                await context.AddAsync(user);
                await context.SaveChangesAsync();

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, new List<ApplicationUser> { user })
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();


                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.RemoveUserFromBudget(user.Id, budget.Id));
            }
        }

        [Test]
        public async Task RemoveUserFromBudget_ThrowException_IfUserNotAssignedToBudget()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var user = new Faker<ApplicationUser>()
                    .RuleFor(x => x.UserName, f => f.Name.FirstName())
                    .Generate();

                var requestingUser = await GetMockedUser(context);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, new List<ApplicationUser> { requestingUser })
                    .Generate();

                await context.AddAsync(user);
                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.RemoveUserFromBudget(user.Id, budget.Id));
            }
        }


        [Test]
        public async Task RemoveUserFromBudget_ThrowException_IfUserTryToRemoveHimself()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var requestingUser = await GetMockedUser(context);

                var budget = new Faker<Budget>()
                    .RuleFor(x => x.Name, f => f.Finance.AccountName())
                    .RuleFor(x => x.Balance, f => f.Random.Decimal())
                    .RuleFor(x => x.UsersAssignedToBudget, new List<ApplicationUser> { requestingUser })
                    .Generate();

                await context.AddAsync(budget);
                await context.SaveChangesAsync();

                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.RemoveUserFromBudget(requestingUser.Id, budget.Id));
            }
        }

        [Test]
        public async Task RemoveUserFromBudget_ThrowException_IfUserNotExists()
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
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.RemoveUserFromBudget("TestId", budget.Id));
            }
        }

        private BudgetService GetSut(ApplicationDbContext context) => new BudgetService(context, UserProvider);
    }
}
