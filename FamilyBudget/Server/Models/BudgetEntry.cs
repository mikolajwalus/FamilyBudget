namespace FamilyBudget.Server.Models
{
    public class BudgetEntry : BaseEntity
    {
        public decimal MoneyAmount { get; set; }
        public Guid BudgetId { get; set; }
        public Guid BudgetEntryCategoryId { get; set; }
        public Budget Budget { get; set; }
        public BudgetEntryCategory BudgetEntryCategory { get; set; }
    }
}
