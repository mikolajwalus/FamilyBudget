using FamilyBudget.Server.Data;
using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.Budget;

namespace FamilyBudget.Server.Services.Budget
{
    public class BudgetEntriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserProvider _userProvider;

        public BudgetEntriesService(ApplicationDbContext context, IUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public async Task<BudgetEntriesDto> GetBudgetEntries(BudgetEntriesRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
