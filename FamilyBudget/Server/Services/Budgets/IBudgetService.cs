using FamilyBudget.Shared.Budget;

namespace FamilyBudget.Server.Services.Budgets
{
    public interface IBudgetService
    {
        Task AddUserToBudget(string userId, Guid budgetId);
        Task<BudgetDto> CreateBudget(BudgetForCreationDto dto);
        Task<BudgetDto> GetBudget(Guid id);
        Task<List<UserForBudget>> GetUsers();
        Task<List<BudgetDto>> GetUserBudgets();
        Task<List<UserForBudget>> GetUsersAssignedToBudget(Guid id);
        Task RemoveUserFromBudget(string userId, Guid budgetId);
        Task UpdateBudget(BudgetForUpdateDto dto);
    }
}