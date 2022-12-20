using FamilyBudget.Shared.BudgetEntries;

namespace FamilyBudget.Client.Services
{
    public interface IBudgetEntriesService
    {
        Task<BudgetEntryDto> CreateEntry(BudgetEntryForCreationDto dto);
        Task DeleteEntry(Guid id);
        Task<BudgetEntriesDto> GetBudgetEntries(BudgetEntriesRequestDto dto);
        Task UpdateEntry(BudgetEntryForUpdateDto dto);
    }
}
