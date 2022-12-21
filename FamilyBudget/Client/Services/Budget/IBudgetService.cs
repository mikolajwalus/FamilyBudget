using FamilyBudget.Shared.Budget;

namespace FamilyBudget.Client.Services
{
    public interface IBudgetService
    {
        Task<BudgetDto> CreateBudget(BudgetForCreationDto dto);
        Task<BudgetDto> GetBudget(Guid id);
        Task<List<UserForBudget>> GetUsers();
        Task<List<BudgetDto>> GetUserBudgets();
        Task<List<UserForBudget>> GetUsersAssignedToBudget(Guid id);
        Task<bool> UpdateBudget(BudgetForUpdateDto dto);
    }
}
