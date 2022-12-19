namespace FamilyBudget.Shared.Budget
{
    public class UserBudgetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
