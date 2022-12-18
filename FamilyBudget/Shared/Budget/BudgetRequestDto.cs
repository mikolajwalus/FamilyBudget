using FamilyBudget.Shared.Enums;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetRequestDto
    {
        public Guid Id { get; set; }
        public BudgetEntriesType EntriesType { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
