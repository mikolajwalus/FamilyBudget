namespace FamilyBudget.Server.Models
{
    public class Budget : BaseEntity
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public ICollection<ApplicationUser> UsersAssignedToBudget { get; set; }
        public ICollection<BudgetEntry> BudgetEntries { get; set; }
    }
}
