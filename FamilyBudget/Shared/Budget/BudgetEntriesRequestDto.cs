using FamilyBudget.Shared.Enums;
using FamilyBudget.Shared.Pagination;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetEntriesRequestDto
    {
        public Guid BudgetId { get; set; }
        public BudgetEntriesType EntriesType { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public Guid? CategoryId { get; set; }
        public PaginationParamsDto PaginationParams { get; set; }
    }
}
