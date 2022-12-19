namespace FamilyBudget.Shared.BudgetEntries
{
    public class BudgetEntryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public decimal MoneyAmount { get; set; }
        public string CategoryName { get; set; }
    }
}
