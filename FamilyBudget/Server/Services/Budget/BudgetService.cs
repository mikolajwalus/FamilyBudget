using FamilyBudget.Server.Data;
using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.Budget;

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

        public async Task<List<UserBudgetDto>> GetUserBudgets()
        {
        }

        public async Task<BudgetDto> GetBudget(BudgetRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
