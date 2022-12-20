using FamilyBudget.Client.Services;
using FamilyBudget.Shared.Budget;
using Microsoft.AspNetCore.Components;

namespace FamilyBudget.Client.Pages
{
    public partial class Budgets : ComponentBase
    {
        [Inject]
        IBudgetService _budgetService { get; set; }
        public List<BudgetDto> UserBudgets;

        protected override async Task OnInitializedAsync()
        {
            UserBudgets = await _budgetService.GetUserBudgets();
        }
    }
}
