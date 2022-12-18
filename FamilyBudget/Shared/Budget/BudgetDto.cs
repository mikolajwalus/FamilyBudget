using FamilyBudget.Shared.Pagination;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PaginationResponseDto EntriesPagination { get; set; }
        public List<BudgetEntryDto> Entries { get; set; }
    }
}
