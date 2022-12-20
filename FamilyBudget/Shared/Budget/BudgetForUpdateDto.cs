using System.ComponentModel.DataAnnotations;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetForUpdateDto
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public List<string> AssignedUsers { get; set; }
    }
}
