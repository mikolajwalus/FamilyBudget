using FamilyBudget.Shared.BudgetEntries;

namespace FamilyBudget.Client.Services
{
    public interface IBudgetEntryCategoriesService
    {
        Task<BudgetEntryCategoryDto> Create(string name);
        Task Delete(Guid id);
        Task<List<BudgetEntryCategoryDto>> GetAll();
        Task<BudgetEntryCategoryDto> Update(BudgetEntryCategoryDto dto);
    }
}
