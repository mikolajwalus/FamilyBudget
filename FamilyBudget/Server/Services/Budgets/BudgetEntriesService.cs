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
            throw new NotImplementedException();
        }

        public async Task UpdateEntry(BudgetEntryForUpdateDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteEntry(Guid id)
        {
            throw new NotImplementedException();
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
    }
}
