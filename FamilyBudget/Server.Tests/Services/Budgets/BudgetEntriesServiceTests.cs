using Bogus;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.BudgetEntries;
using FamilyBudget.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FamilyBudget.Server.Tests.Services.Budgets
{
    [TestFixture]
    public class BudgetEntriesServiceTests : ServiceTest
    {
        [Test]
        public void CreateEntry_ThrowException_IfBudgetNotExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var dto = new BudgetEntryForCreationDto
                {
                    BudgetId = Guid.NewGuid(),
                    MoneyAmount = 10,
                };

                //Act and assert
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.CreateEntry(dto));
            }
        }

        [Test]
        public async Task CreateEntry_ThrowException_IfUserIsNotAssignedToBudget()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                var dto = new BudgetEntryForCreationDto
                {
                    BudgetId = budget.Id,
                    MoneyAmount = 10,
                };

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.CreateEntry(dto));
            }
        }

        [Test]
        public async Task CreateEntry_ThrowException_IfMoneyAmountIsZero()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                await AssignBudgetToUser(context, budget);

                var categoryId = (await CreateCategories(context)).First().Id;

                var dto = new BudgetEntryForCreationDto
                {
                    BudgetId = budget.Id,
                    MoneyAmount = 0,
                    CategoryId = categoryId
                };

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.CreateEntry(dto));
            }
        }

        [Test]
        public async Task CreateEntry_ThrowException_IfCategoryDontExist()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                await AssignBudgetToUser(context, budget);

                var dto = new BudgetEntryForCreationDto
                {
                    BudgetId = budget.Id,
                    MoneyAmount = 0,
                    CategoryId = Guid.NewGuid()
                };

                //Act and assert
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.CreateEntry(dto));
            }
        }

        [Test]
        public async Task CreateEntry_CreateEntryAndUpdatesBudgetBalance()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                await AssignBudgetToUser(context, budget);

                var category = (await CreateCategories(context)).First();


                var dto = new BudgetEntryForCreationDto
                {
                    BudgetId = budget.Id,
                    MoneyAmount = 10,
                    CategoryId = category.Id
                };

                var expectedBudgetBalance = budget.Balance + dto.MoneyAmount;

                //Act
                var result = await sut.CreateEntry(dto);

                var entryFromDb = await context.BudgetEntries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == result.Id);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == dto.BudgetId);

                //Assert
                Assert.AreEqual(expectedBudgetBalance, budgetFromDb.Balance);

                Assert.AreEqual(dto.MoneyAmount, entryFromDb.MoneyAmount);
                Assert.AreEqual(dto.CategoryId, entryFromDb.BudgetEntryCategoryId);
                Assert.AreEqual(dto.BudgetId, entryFromDb.BudgetId);

                Assert.AreEqual(dto.MoneyAmount, result.MoneyAmount);
                Assert.AreEqual(category.Name, result.CategoryName);
            }
        }

        [Test]
        public void UpdateEntry_ThrowException_IfEntryNotExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var dto = new BudgetEntryForUpdateDto
                {
                    Id = Guid.NewGuid(),
                    MoneyAmount = 10,
                };

                //Act and assert
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.UpdateEntry(dto));
            }
        }

        [Test]
        public async Task UpdateEntry_ThrowException_IfUserIsNotAssignedToBudget()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                var existingEntry = await CreateEntryWithCategory(context, budget.Id);

                var dto = new BudgetEntryForUpdateDto
                {
                    Id = existingEntry.Id,
                    MoneyAmount = 10,
                };

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.UpdateEntry(dto));
            }
        }

        [Test]
        public async Task UpdateEntry_ThrowException_IfMoneyAmountIsZero()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                await AssignBudgetToUser(context, budget);

                var existingEntry = await CreateEntryWithCategory(context, budget.Id);

                var dto = new BudgetEntryForUpdateDto
                {
                    Id = existingEntry.Id,
                    MoneyAmount = 0,
                    CategoryId = existingEntry.BudgetEntryCategoryId
                };

                //Act and assert
                Assert.ThrowsAsync<BadRequestException>(async () => await sut.UpdateEntry(dto));
            }
        }

        [Test]
        public async Task UpdateEntry_ThrowException_IfCategoryDontExist()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                await AssignBudgetToUser(context, budget);

                var existingEntry = await CreateEntryWithCategory(context, budget.Id);

                var dto = new BudgetEntryForUpdateDto
                {
                    Id = existingEntry.Id,
                    MoneyAmount = 10,
                    CategoryId = Guid.NewGuid()
                };

                //Act and assert
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.UpdateEntry(dto));
            }
        }

        [Test]
        public async Task UpdateEntry_UpdatesEntryAndBudgetBalance()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                await AssignBudgetToUser(context, budget);

                var existingEntry = await CreateEntryWithCategory(context, budget.Id);

                var newCategory = (await CreateCategories(context)).First();

                var dto = new BudgetEntryForUpdateDto
                {
                    Id = existingEntry.Id,
                    MoneyAmount = 10,
                    CategoryId = newCategory.Id
                };

                var expectedBudgetBalance = budget.Balance - existingEntry.MoneyAmount + dto.MoneyAmount;

                //Act
                await sut.UpdateEntry(dto);

                var entryFromDb = await context.BudgetEntries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == existingEntry.Id);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == budget.Id);

                //Assert
                Assert.AreEqual(expectedBudgetBalance, budgetFromDb.Balance);

                Assert.AreEqual(dto.MoneyAmount, entryFromDb.MoneyAmount);
                Assert.AreEqual(dto.CategoryId, entryFromDb.BudgetEntryCategoryId);
            }
        }

        [Test]
        public void DeleteEntry_ThrowException_IfEntryNotExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                //Act and assert
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.DeleteEntry(Guid.NewGuid()));
            }
        }

        [Test]
        public async Task DeleteEntry_ThrowException_IfUserIsNotAssignedToBudget()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                var existingEntry = await CreateEntryWithCategory(context, budget.Id);

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.DeleteEntry(existingEntry.Id));
            }
        }

        [Test]
        public async Task DeleteEntry_DeletesEntryAndUpdateBudgetBalance()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var budget = await CreateBudget(context);

                var existingEntry = await CreateEntryWithCategory(context, budget.Id);

                await AssignBudgetToUser(context, budget);

                var expectedBudgetBalance = budget.Balance - existingEntry.MoneyAmount;

                //Act
                await sut.DeleteEntry(existingEntry.Id);

                var entryExists = await context.BudgetEntries
                    .AnyAsync(x => x.Id == existingEntry.Id);

                var budgetFromDb = await context.Budgets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == budget.Id);

                //Assert
                Assert.AreEqual(expectedBudgetBalance, budgetFromDb.Balance);
                Assert.IsFalse(entryExists);
            }
        }


        [Test]
        public async Task GetBudgetEntries_ThrowException_IfExistsButIsNotUsers()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var budget = await CreateBudget(context);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                    PageNumber = 1,
                    PageSize = 1
                };

                //Act and assert
                Assert.ThrowsAsync<UnauthorizedException>(async () => await sut.GetBudgetEntries(dto));
            }
        }

        [Test]
        public void GetBudgetEntries_ThrowException_IfBudgetNotExists()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = Guid.NewGuid(),
                };

                AddPaginationParams(dto);

                //Act and assert
                Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sut.GetBudgetEntries(dto));
            }
        }

        [Test]
        public async Task GetBudgetEntries_ReturnsEntriesOrderedByUpdatedAt()
        {
            using (var context = GetDbContext(true))
            {
                //Arrange
                var entriesAmount = 10;

                var budget = await CreateBudget(context);
                var categories = await CreateCategories(context);
                await AssignBudgetToUser(context, budget);

                var entries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => f.Random.Decimal())
                    .RuleFor(x => x.BudgetEntryCategoryId, f => f.PickRandom(categories).Id)
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.CreatedAt, f => f.Date.Recent(10))
                    .RuleFor(x => x.UpdatedAt, (f, u) => u.CreatedAt.AddDays(1))
                    .Generate(entriesAmount);

                await AddEntriesToDb(context, entries);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                };

                AddPaginationParams(dto, 1, entriesAmount);

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(entriesAmount, result.Entries.Count);
                Assert.That(result.Entries, Is.Ordered.By(nameof(BudgetEntryDto.LastUpdatedAt)).Descending);

                foreach (var entry in result.Entries)
                {
                    var category = categories.FirstOrDefault(x => x.Name == entry.CategoryName);

                    Assert.That(entries.Any(x => x.Id == entry.Id && x.BudgetEntryCategoryId == category.Id));
                }
            }
        }

        [Test]
        public async Task GetBudgetEntries_ReturnedFilteredByCategoryId_IfFiletringRequested()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var entriesToReturnAmount = 10;
                var otherEntriesAmount = 50;

                var budget = await CreateBudget(context);
                var requestedCategory = (await CreateCategories(context, 1)).First();
                var otherCategories = await CreateCategories(context, 1);

                await AssignBudgetToUser(context, budget);

                var entriesWithPropperCategory = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => f.Random.Decimal())
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.BudgetEntryCategoryId, requestedCategory.Id)
                    .Generate(entriesToReturnAmount);

                var entriesWithOtherCategory = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => f.Random.Decimal())
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.BudgetEntryCategoryId, f => f.PickRandom(otherCategories).Id)
                    .Generate(otherEntriesAmount);

                await AddEntriesToDb(context, entriesWithPropperCategory);
                await AddEntriesToDb(context, entriesWithOtherCategory);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                    CategoryId = requestedCategory.Id,
                };

                AddPaginationParams(dto, 1, entriesToReturnAmount);

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(entriesToReturnAmount, result.Entries.Count);
                Assert.IsTrue(result.Entries.All(x => x.CategoryName == requestedCategory.Name));
            }
        }

        [Test]
        [TestCase(BudgetEntriesType.OnlyIncomes, 10, 15, 10)]
        [TestCase(BudgetEntriesType.OnlyExpenses, 10, 15, 15)]
        [TestCase(BudgetEntriesType.All, 10, 15, 25)]
        public async Task GetBudgetEntries_ReturnedFilteredByAmountValue_IfFiletringRequested(
            BudgetEntriesType entryType,
            int incomeEntriesAmount,
            int expenseEntriesAmount,
            int expectedEntriesAmount)
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var pageSize = incomeEntriesAmount + expenseEntriesAmount;

                var budget = await CreateBudget(context);
                var categoryId = (await CreateCategories(context)).First().Id;

                await AssignBudgetToUser(context, budget);

                var incomeEntries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => GetPositiveDecimal())
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.BudgetEntryCategoryId, categoryId)
                    .Generate(incomeEntriesAmount);

                var expenseEntries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => GetNegativeDecimal())
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.BudgetEntryCategoryId, categoryId)
                    .Generate(expenseEntriesAmount);

                await AddEntriesToDb(context, incomeEntries);
                await AddEntriesToDb(context, expenseEntries);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                    EntriesType = entryType
                };

                AddPaginationParams(dto, 1, pageSize);

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(expectedEntriesAmount, result.Entries.Count);
                Assert.IsTrue(result.Entries.All(GetPredicateBaseOnEntryType(entryType)));
            }
        }

        [Test]
        [TestCase(20, 5, 2, 5)]
        [TestCase(4, 5, 1, 4)]
        [TestCase(12, 5, 3, 2)]
        public async Task GetBudgetEntries_ReturnedPagedValues_WhenPageInRange(
            int entriesAmount,
            int pageSize,
            int pageNumber,
            int expectedEntriesAmount)
        {
            using (var context = GetDbContext(true))
            {
                //Arrange
                var expectedPagesAmount = Math.Ceiling(entriesAmount / (double)pageSize);

                var budget = await CreateBudget(context);
                var categoryId = (await CreateCategories(context)).First().Id;

                await AssignBudgetToUser(context, budget);

                var entries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => f.Random.Decimal())
                    .RuleFor(x => x.BudgetEntryCategoryId, categoryId)
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.CreatedAt, f => f.Date.Recent(15))
                    .RuleFor(x => x.UpdatedAt, (f, u) => u.CreatedAt.AddDays(1))
                    .Generate(entriesAmount);

                await AddEntriesToDb(context, entries);

                var entriesToReturn = entries
                    .OrderByDescending(x => x.UpdatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                };

                AddPaginationParams(dto, pageNumber, pageSize);

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(expectedEntriesAmount, result.Entries.Count);
                Assert.AreEqual(expectedPagesAmount, result.EntriesPagination.TotalPages);
                Assert.AreEqual(pageNumber, result.EntriesPagination.CurrentPage);
                Assert.AreEqual(pageSize, result.EntriesPagination.PageSize);
                Assert.AreEqual(entriesAmount, result.EntriesPagination.TotalCount);

                for (int index = 0; index < entriesToReturn.Count; index++)
                {
                    var entryDto = result.Entries[index];
                    var entry = entriesToReturn[index];

                    Assert.AreEqual(entry.Id, entryDto.Id);
                }
            }
        }

        [Test]
        public async Task GetBudgetEntries_ReturnEmptyList_WhenPageOutOfRange()
        {
            using (var context = GetDbContext(true))
            {
                //Arrange
                var budget = await CreateBudget(context);
                var categoryId = (await CreateCategories(context)).First().Id;

                await AssignBudgetToUser(context, budget);

                var entries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => f.Random.Decimal())
                    .RuleFor(x => x.BudgetEntryCategoryId, categoryId)
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.UpdatedAt, f => f.Date.Recent(15))
                    .Generate(10);

                await AddEntriesToDb(context, entries);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                };

                AddPaginationParams(dto, 2, 15);

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(0, result.Entries.Count);
            }
        }

        [Test]
        public async Task GetBudgetEntries_ThrowsException_WhenPageIsZero()
        {
            using (var context = GetDbContext(true))
            {
                //Arrange
                var budget = await CreateBudget(context);
                var categoryId = (await CreateCategories(context)).First().Id;

                await AssignBudgetToUser(context, budget);

                var entries = new Faker<BudgetEntry>()
                    .RuleFor(x => x.MoneyAmount, f => f.Random.Decimal())
                    .RuleFor(x => x.BudgetEntryCategoryId, categoryId)
                    .RuleFor(x => x.BudgetId, budget.Id)
                    .RuleFor(x => x.UpdatedAt, f => f.Date.Recent(15))
                    .Generate(10);

                await AddEntriesToDb(context, entries);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                };

                AddPaginationParams(dto, 0, 15);

                //Act and assert
                Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetBudgetEntries(dto));
            }
        }

        private static Func<BudgetEntryDto, bool> GetPredicateBaseOnEntryType(BudgetEntriesType entryType)
        {
            if (entryType == BudgetEntriesType.OnlyIncomes)
            {
                return x => x.MoneyAmount > 0;
            }

            if (entryType == BudgetEntriesType.OnlyExpenses)
            {
                return x => x.MoneyAmount < 0;
            }

            return x => true;
        }

        private static decimal GetNegativeDecimal()
        {
            var f = new Faker();

            var value = f.Random.Decimal(Decimal.MinValue, 0);

            return value == 0 ? value - 1 : value;
        }

        private static decimal GetPositiveDecimal()
        {
            var f = new Faker();

            var value = f.Random.Decimal(0, Decimal.MaxValue);

            return value == 0 ? value + 1 : value;
        }

        private BudgetEntriesService GetSut(ApplicationDbContext context) => new BudgetEntriesService(context, UserProvider);
        private async Task<Budget> CreateBudget(ApplicationDbContext context)
        {
            var budget = new Faker<Budget>()
                .RuleFor(x => x.Name, f => f.Finance.AccountName())
                .RuleFor(x => x.UsersAssignedToBudget, new List<ApplicationUser>())
                .Generate();

            await context.AddAsync(budget);
            await context.SaveChangesAsync();

            return budget;
        }

        private async Task AssignBudgetToUser(ApplicationDbContext context, Budget budget)
        {
            var user = await GetMockedUser(context);

            budget.UsersAssignedToBudget.Add(user);

            await context.SaveChangesAsync();
        }

        private async Task<BudgetEntry> CreateEntryWithCategory(ApplicationDbContext context, Guid budgetId)
        {
            var categoryId = (await CreateCategories(context)).First().Id;

            var entry = new BudgetEntry
            {
                BudgetId = budgetId,
                BudgetEntryCategoryId = categoryId,
                MoneyAmount = 100,
            };

            await context.AddAsync(entry);
            await context.SaveChangesAsync();

            return entry;
        }

        private async Task AddEntriesToDb(ApplicationDbContext context, List<BudgetEntry> entries)
        {
            await context.AddRangeAsync(entries);
            await context.SaveChangesAsync();
        }
        private async Task<List<BudgetEntryCategory>> CreateCategories(ApplicationDbContext context, int amount = 1)
        {
            var categories = new Faker<BudgetEntryCategory>()
                .RuleFor(x => x.Name, f => f.Finance.AccountName())
                .Generate(amount);

            await context.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            return categories;
        }

        private void AddPaginationParams(BudgetEntriesRequestDto dto, int pageNumber = 1, int pageSize = 1)
        {
            dto.PageNumber = pageNumber;
            dto.PageSize = pageSize;
        }
    }
}
