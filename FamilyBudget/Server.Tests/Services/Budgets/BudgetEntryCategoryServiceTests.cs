using Bogus;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Infractructure.Configuration;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.BudgetEntries;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FamilyBudget.Server.Tests.Services.Budgets
{

    [TestFixture]
    public class BudgetEntryCategoryServiceTests
    {
        private BudgetEntryCategoriesService _sut;
        private ApplicationDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;
            var dbContextOptions = Options.Create(new DbContextConfiguration());
            var storeOptions = Options.Create(new OperationalStoreOptions());

            _context = new ApplicationDbContext(options, dbContextOptions, storeOptions);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _sut = new BudgetEntryCategoriesService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Create_ShouldAddCategory_WhenValidCategoryIsProvided()
        {

            // Arrange
            var name = "Food";

            // Act
            var result = await _sut.Create(name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(1, _context.BudgetEntryCategories.Count());
        }

        [Test]
        public async Task Create_ShouldThrowException_WhenCategoryWithSameNameExists()
        {
            // Arrange
            var name = "Food";
            _context.BudgetEntryCategories.Add(new BudgetEntryCategory { Name = name });
            await _context.SaveChangesAsync();

            // Act and Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _sut.Create(name));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCategories_WhenCategoriesExist()
        {
            // Arrange
            int categoriesAmount = 5;

            var categories = new Faker<BudgetEntryCategory>()
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .Generate(categoriesAmount);

            _context.BudgetEntryCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(categoriesAmount, result.Count);

        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateCategory_WhenCategoryExists()
        {
            // Arrange
            var category = new BudgetEntryCategory { Name = "Food" };
            _context.BudgetEntryCategories.Add(category);
            await _context.SaveChangesAsync();


            var dto = new BudgetEntryCategoryDto { Id = category.Id, Name = "Groceries" };

            // Act
            var result = await _sut.Update(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result.Name);
        }


        [Test]
        public void UpdateAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var dto = new BudgetEntryCategoryDto { Id = Guid.NewGuid(), Name = "Groceries" };

            // Act and Assert
            var ex = Assert.ThrowsAsync<ResourceNotFoundException>(() => _sut.Update(dto));
        }

        [Test]
        public async Task UpdateAsync_ShouldThrowException_WhenCategoryWithSameNameExists()
        {
            // Arrange
            var categoryToUpdate = new BudgetEntryCategory { Name = "Groceries" };
            await _context.BudgetEntryCategories.AddAsync(categoryToUpdate);

            var categoryWithTheSameName = new BudgetEntryCategory { Name = "Food" };
            await _context.BudgetEntryCategories.AddAsync(categoryWithTheSameName);

            await _context.SaveChangesAsync();

            var dto = new BudgetEntryCategoryDto { Id = categoryToUpdate.Id, Name = "Food" };

            // Act and Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(() => _sut.Update(dto));
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteCategory_WhenCategoryExists()
        {
            // Arrange
            var category = new BudgetEntryCategory { Name = "Food" };
            _context.BudgetEntryCategories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            await _sut.Delete(category.Id);

            // Assert
            Assert.AreEqual(0, _context.BudgetEntryCategories.Count());
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Act and Assert
            var ex = Assert.ThrowsAsync<ResourceNotFoundException>(() => _sut.Delete(Guid.NewGuid()));
        }

    }
}