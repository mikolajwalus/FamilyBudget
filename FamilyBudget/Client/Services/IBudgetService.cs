using FamilyBudget.Shared.Budget;

namespace FamilyBudget.Client.Services
{
    public interface IBudgetService
    {
        Task AddUserToBudget(string userId, Guid budgetId);
        Task<BudgetDto> CreateBudget(BudgetForCreationDto dto);
        Task<BudgetDto> GetBudget(Guid id);
        Task<List<BudgetDto>> GetUserBudgets();
        Task<List<UserAssignedToBudgetDto>> GetUsersAssignedToBudget(Guid id);
        Task RemoveUserFromBudget(string userId, Guid budgetId);
        Task UpdateBudget(BudgetForUpdateDto dto);
    }
}
