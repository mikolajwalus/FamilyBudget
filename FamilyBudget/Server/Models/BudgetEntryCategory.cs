namespace FamilyBudget.Server.Models
{
    public class BudgetEntryCategory : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<BudgetEntry> BudgetEntries { get; set; }
    }
}
