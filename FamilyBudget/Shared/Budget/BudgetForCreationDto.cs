using System.ComponentModel.DataAnnotations;

namespace FamilyBudget.Shared.Budget
{
    public class BudgetForCreationDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public List<string> AssignedUsers { get; set; }
    }
}
