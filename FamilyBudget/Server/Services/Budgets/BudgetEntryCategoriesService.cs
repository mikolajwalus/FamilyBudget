using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Shared.BudgetEntries;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Services.Budgets
{
    public class BudgetEntryCategoriesService : IBudgetEntryCategoriesService
    {
        private readonly ApplicationDbContext _context;

        public BudgetEntryCategoriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BudgetEntryCategoryDto> Create(string name)
        {
            if (await _context.BudgetEntryCategories.AnyAsync(c => c.Name == name))
            {
                throw new BadRequestException(ResponseMessages.GetBudgetEntryCategoryWithNameExistsMessage(name));
            }

            var category = new BudgetEntryCategory { Name = name };
            _context.BudgetEntryCategories.Add(category);
            await _context.SaveChangesAsync();

            return new BudgetEntryCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<List<BudgetEntryCategoryDto>> GetAll()
        {
            return await _context.BudgetEntryCategories
                .Select(x => new BudgetEntryCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();
        }

        public async Task<BudgetEntryCategoryDto> Update(BudgetEntryCategoryDto dto)
        {
            var category = await _context.BudgetEntryCategories.FindAsync(dto.Id);
            if (category is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetCategoryNotExistsMessage(dto.Id));
            }

            if (await _context.BudgetEntryCategories.AnyAsync(c => c.Name == dto.Name && c.Id != dto.Id))
            {
                throw new BadRequestException(ResponseMessages.GetBudgetEntryCategoryWithNameExistsMessage(dto.Name));
            }

            category.Name = dto.Name;
            _context.BudgetEntryCategories.Update(category);
            await _context.SaveChangesAsync();

            return new BudgetEntryCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task Delete(Guid id)
        {
            var category = await _context.BudgetEntryCategories.FindAsync(id);

            if (category is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetCategoryNotExistsMessage(id));
            }

            _context.BudgetEntryCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
