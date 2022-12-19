using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.BudgetEntries;
using FamilyBudget.Shared.Enums;
using FamilyBudget.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Services.Budgets
{
    public class BudgetEntriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _requestingUserId;

        public BudgetEntriesService(ApplicationDbContext context, IUserProvider userProvider)
        {
            _context = context;
            _requestingUserId = userProvider.UserId;
        }

        public async Task<BudgetEntryDto> CreateEntry(BudgetEntryForCreationDto dto)
        {
            await ValidateForCreation(dto);

            var budget = await _context.Budgets.FindAsync(dto.BudgetId);
            var category = await _context.BudgetEntryCategories.FindAsync(dto.CategoryId);

            budget.Balance += dto.MoneyAmount;

            var budgetEntry = new BudgetEntry
            {
                MoneyAmount = dto.MoneyAmount,
                BudgetEntryCategoryId = dto.CategoryId,
                BudgetId = dto.BudgetId,
            };

            await _context.AddAsync(budgetEntry);
            await _context.SaveChangesAsync();

            return new BudgetEntryDto
            {
                Id = budgetEntry.Id,
                MoneyAmount = budgetEntry.MoneyAmount,
                CreatedAt = budgetEntry.CreatedAt,
                LastUpdatedAt = budgetEntry.UpdatedAt,
                CategoryName = category.Name,
            };
        }

        public async Task UpdateEntry(BudgetEntryForUpdateDto dto)
        {
            var entry = await GetEntryWithBudgetAndUser(dto.Id);

            await ValidateForUpdate(dto, entry);

            entry.Budget.Balance = entry.Budget.Balance - entry.MoneyAmount + dto.MoneyAmount;

            entry.BudgetEntryCategoryId = dto.CategoryId;
            entry.MoneyAmount = dto.MoneyAmount;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteEntry(Guid id)
        {
            var entry = await GetEntryWithBudgetAndUser(id);

            ValidateForDelete(id, entry);

            entry.Budget.Balance -= entry.MoneyAmount;

            _context.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<BudgetEntriesDto> GetBudgetEntries(BudgetEntriesRequestDto dto)
        {
            if (dto.PaginationParams is null)
            {
                throw new ArgumentNullException(nameof(dto.PaginationParams));
            }

            if (dto.PaginationParams.PageNumber == 0)
            {
                throw new ArgumentException(nameof(dto.PaginationParams.PageSize));
            }

            var budgetId = dto.BudgetId;

            await CheckIfBudgetExistsAndIsAssignedToUser(_requestingUserId, budgetId);

            var entriesQuery = _context.BudgetEntries
                .Include(x => x.BudgetEntryCategory)
                .Where(x => x.BudgetId == budgetId);

            entriesQuery = ImplementFiltering(dto, entriesQuery);

            entriesQuery = ImplementPagination(dto, entriesQuery);

            var entries = await GetDtos(entriesQuery);

            var paginationResponseDto = await GetPaginationResponse(dto);

            return new BudgetEntriesDto
            {
                EntriesPagination = paginationResponseDto,
                Entries = entries
            };
        }

        private async Task ValidateForCreation(BudgetEntryForCreationDto dto)
        {
            await CheckIfBudgetExistsAndIsAssignedToUser(_requestingUserId, dto.BudgetId);
            await CheckIfCategoryExists(dto.CategoryId);
            CheckIfMoneyAmountNotEqualsZero(dto.MoneyAmount);
        }

        private async Task ValidateForUpdate(BudgetEntryForUpdateDto dto, BudgetEntry entry)
        {
            CheckIfEntryExists(entry, dto.Id);
            CheckIfBudgetAssignedToUser(entry.Budget, _requestingUserId);

            if (dto.CategoryId != entry.BudgetEntryCategoryId)
            {
                await CheckIfCategoryExists(dto.CategoryId);
            }

            CheckIfMoneyAmountNotEqualsZero(dto.MoneyAmount);
        }

        private void ValidateForDelete(Guid entryId, BudgetEntry entry)
        {
            CheckIfEntryExists(entry, entryId);
            CheckIfBudgetAssignedToUser(entry.Budget, _requestingUserId);
        }

        private async Task CheckIfCategoryExists(Guid categoryId)
        {
            var categoryExists = await _context.BudgetEntryCategories.AnyAsync(x => x.Id == categoryId);

            if (!categoryExists)
            {
                throw new NotFoundException(ResponseMessages.GetCategoryNotExistsMessage(categoryId));
            }
        }

        private void CheckIfMoneyAmountNotEqualsZero(decimal moneyAmount)
        {
            if (moneyAmount == 0)
            {
                throw new BadRequestException(ResponseMessages.BudgetEntryMoneyNotZero);
            }
        }

        private void CheckIfEntryExists(BudgetEntry entry, Guid entryId)
        {
            if (entry is null)
            {
                throw new NotFoundException(ResponseMessages.GetBudgetEntryNotExistsMessage(entryId));
            }
        }

        private void CheckIfBudgetAssignedToUser(Budget budget, string userId)
        {
            if (!budget.UsersAssignedToBudget.Any(x => x.Id == userId))
            {
                throw new UnauthorizedException(ResponseMessages.GetGetUserNotAssignedToBudgetMessage(budget.Id, userId));
            }
        }

        private async Task<PaginationResponseDto> GetPaginationResponse(BudgetEntriesRequestDto dto)
        {
            var itemsAmount = await _context.BudgetEntries.CountAsync();

            var paginationResponseDto = new PaginationResponseDto(itemsAmount, dto.PaginationParams.PageNumber, dto.PaginationParams.PageSize);
            return paginationResponseDto;
        }

        private static async Task<List<BudgetEntryDto>> GetDtos(IQueryable<BudgetEntry> entriesQuery)
        {
            return await entriesQuery
                .Select(x => new BudgetEntryDto
                {
                    Id = x.Id,
                    MoneyAmount = x.MoneyAmount,
                    CategoryName = x.BudgetEntryCategory.Name
                })
                .ToListAsync();
        }

        private static IQueryable<BudgetEntry> ImplementPagination(BudgetEntriesRequestDto dto, IQueryable<BudgetEntry> entriesQuery)
        {
            entriesQuery = entriesQuery.OrderBy(x => x.UpdatedAt);

            entriesQuery = entriesQuery
                .Skip((dto.PaginationParams.PageNumber - 1) * dto.PaginationParams.PageSize)
                .Take(dto.PaginationParams.PageSize);

            return entriesQuery;
        }

        private static IQueryable<BudgetEntry> ImplementFiltering(BudgetEntriesRequestDto dto, IQueryable<BudgetEntry> entriesQuery)
        {
            if (dto.EntriesType == BudgetEntriesType.OnlyIncomes)
            {
                entriesQuery = entriesQuery.Where(x => x.MoneyAmount > 0);
            }

            if (dto.EntriesType == BudgetEntriesType.OnlyExpenses)
            {
                entriesQuery = entriesQuery.Where(x => x.MoneyAmount < 0);
            }

            if (dto.CategoryId is not null)
            {
                entriesQuery = entriesQuery.Where(x => x.BudgetEntryCategoryId == dto.CategoryId);
            }

            return entriesQuery;
        }

        private async Task CheckIfBudgetExistsAndIsAssignedToUser(string userId, Guid budgetId)
        {
            var budget = await _context.Budgets
                .Include(x => x.UsersAssignedToBudget.Where(x => x.Id == userId))
                .Where(x => x.Id == budgetId)
                .FirstOrDefaultAsync();

            if (budget is null)
            {
                throw new NotFoundException(ResponseMessages.GetBudgetNotExistsMessage(budgetId));
            }

            if (budget.UsersAssignedToBudget.Count == 0)
            {
                throw new UnauthorizedException(ResponseMessages.GetGetUserNotAssignedToBudgetMessage(budgetId, _requestingUserId));
            }
        }

        private async Task<BudgetEntry> GetEntryWithBudgetAndUser(Guid entryId)
        {
            return await _context.BudgetEntries
                .Include(x => x.Budget)
                .ThenInclude(x => x.UsersAssignedToBudget.Where(u => u.Id == _requestingUserId))
                .FirstOrDefaultAsync(x => x.Id == entryId);
        }
    }
}
