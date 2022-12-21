namespace FamilyBudget.Shared.BudgetEntries
{
    public class BudgetEntryCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
