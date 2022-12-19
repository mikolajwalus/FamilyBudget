using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.Budget;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Services.Budget
{
    public class BudgetService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserProvider _userProvider;

        public BudgetService(ApplicationDbContext context, IUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public async Task<List<BudgetDto>> GetUserBudgets()
        {
            var userId = _userProvider.UserId;

            var user = await _context.Users
                .Include(x => x.UserBudgets)
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                throw new UserNotExistException(userId);
            }

            return user.UserBudgets.Select(x => new BudgetDto()
            {
                Id = x.Id,
                Balance = x.Balance,
                Name = x.Name,
            })
            .ToList();
        }

        public async Task<BudgetDto> GetBudgetDto(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<BudgetEntriesDto> GetBudgetEntries(BudgetEntriesRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
