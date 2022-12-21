using FamilyBudget.Shared.Enums;

namespace FamilyBudget.Shared.BudgetEntries
{
    public class BudgetEntriesRequestDto
    {
        public Guid BudgetId { get; set; }
        public BudgetEntriesType EntriesType { get; set; }
        public Guid? CategoryId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
