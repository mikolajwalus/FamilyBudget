using FamilyBudget.Shared.Pagination;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetEntriesDto
    {
        public PaginationResponseDto EntriesPagination { get; set; }
        public List<BudgetEntryDto> Entries { get; set; }
    }
}
