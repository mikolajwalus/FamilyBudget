using FamilyBudget.Shared.Enums;
using FamilyBudget.Shared.Pagination;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetEntriesRequestDto
    {
        public Guid BudgetId { get; set; }
        public BudgetEntriesType EntriesType { get; set; }
        public Guid? CategoryId { get; set; }
        [Required]
        public PaginationParamsDto PaginationParams { get; set; }
    }
}
