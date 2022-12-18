using Microsoft.AspNetCore.Identity;

namespace FamilyBudget.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Budget> UserBudgets { get; set; }
    }
}