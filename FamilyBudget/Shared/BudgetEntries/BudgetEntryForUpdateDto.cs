namespace FamilyBudget.Shared.BudgetEntries
{
    public class BudgetEntryForUpdateDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public decimal MoneyAmount { get; set; }
    }
}
