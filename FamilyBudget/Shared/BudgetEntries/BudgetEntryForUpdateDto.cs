namespace FamilyBudget.Shared.BudgetEntries
{
    public class BudgetEntryForUpdateDto
    {
        public Guid Id { get; set; }
        public Guid BudgetId { get; set; }
        public decimal Amount { get; set; }
    }
}
