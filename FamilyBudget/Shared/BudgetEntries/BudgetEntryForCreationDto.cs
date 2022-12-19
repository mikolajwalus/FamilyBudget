namespace FamilyBudget.Shared.BudgetEntries
{
    public class BudgetEntryForCreationDto
    {
        public Guid BudgetId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal MoneyAmount { get; set; }
    }
}
