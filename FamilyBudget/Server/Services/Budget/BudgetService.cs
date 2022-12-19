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

        public async Task AddUserToBudget(string id)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserFromBudget(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserAssignedToBudgetDto>> GetUsersAssignedToBudget(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<BudgetDto> CreateBudget(BudgetForCreationDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBudget(BudgetForUpdateDto dto)
        {
            throw new NotImplementedException();
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

        public async Task<BudgetDto> GetBudget(Guid id)
        {
            var userId = _userProvider.UserId;

            var budget = await _context.Budgets
                .Where(x => x.UsersAssignedToBudget.Any(user => user.Id == userId) && x.Id == id)
                .FirstOrDefaultAsync();

            if (budget is null)
            {
                throw new BudgetNotExistException(id);
            }

            return new BudgetDto
            {
                Id = id,
                Balance = budget.Balance,
                Name = budget.Name,
            };
        }
    }
}
