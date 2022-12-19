using Bogus;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budget;
using FamilyBudget.Shared.Budget;
using FamilyBudget.Shared.BudgetEntries;
using FamilyBudget.Shared.Enums;
using FamilyBudget.Shared.Pagination;
using NUnit.Framework;

namespace FamilyBudget.Server.Tests.Services.Budgets
{
    [TestFixture]
    public class BudgetEntriesServiceTests : ServiceTest
    {
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
                    PaginationParams = new PaginationParamsDto()
                    {
                        PageNumber = 1,
                        PageSize = 1
                    }
                };

                //Act and assert
                Assert.ThrowsAsync<BudgetNotExistException>(async () => await sut.GetBudgetEntries(dto));
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
                    PaginationParams = GetPaginationParams()
                };

                //Act and assert
                Assert.ThrowsAsync<BudgetNotExistException>(async () => await sut.GetBudgetEntries(dto));
            }
        }

        [Test]
        public async Task GetBudgetEntries_ThrowException_IfPaginationParamsAreNull()
        {
            using (var context = GetDbContext())
            {
                //Arrange
                var budget = await CreateBudget(context);
                await AssignBudgetToUser(context, budget);

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = Guid.NewGuid(),
                };

                //Act and assert
                Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetBudgetEntries(dto));
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
                    PaginationParams = GetPaginationParams(1, entriesAmount),
                };

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(entriesAmount, result.Entries.Count);
                Assert.That(result.Entries, Is.Ordered.By(nameof(BudgetEntryDto.LastUpdatedAt)));

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
                    PaginationParams = GetPaginationParams(1, entriesToReturnAmount),
                    CategoryId = requestedCategory.Id,
                };

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
                    PaginationParams = GetPaginationParams(1, pageSize),
                    EntriesType = entryType
                };

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
                    .OrderBy(x => x.UpdatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var sut = GetSut(context);

                var dto = new BudgetEntriesRequestDto
                {
                    BudgetId = budget.Id,
                    PaginationParams = GetPaginationParams(pageNumber, pageSize),
                };

                //Act
                var result = await sut.GetBudgetEntries(dto);

                //Assert
                Assert.AreEqual(expectedEntriesAmount, result.Entries.Count);
                Assert.AreEqual(expectedPagesAmount, result.EntriesPagination.TotalPages);
                Assert.AreEqual(pageNumber, result.EntriesPagination.CurrentPage);
                Assert.AreEqual(pageSize, result.EntriesPagination.PageSize);

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
                    PaginationParams = GetPaginationParams(2, 15),
                };

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
                    PaginationParams = GetPaginationParams(0, 15),
                };

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

        private PaginationParamsDto GetPaginationParams(int pageNumber = 1, int pageSize = 1) => new PaginationParamsDto()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
