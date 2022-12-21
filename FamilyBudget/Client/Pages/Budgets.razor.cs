using FamilyBudget.Client.Services;
using FamilyBudget.Shared.Budget;
using Microsoft.AspNetCore.Components;

namespace FamilyBudget.Client.Pages
{
    public partial class Budgets : ComponentBase
    {
        [Inject]
        IBudgetService BudgetService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public List<BudgetDto> UserBudgets;

        protected override async Task OnInitializedAsync()
        {
            UserBudgets = await BudgetService.GetUserBudgets();
        }

        void Edit(Guid id)
        {
            NavigationManager.NavigateTo($"/Budget/{id}");
        }

        void ToDashboard(Guid id)
        {
            NavigationManager.NavigateTo($"/BudgetDashboard/{id}");
        }

        void Add()
        {
            NavigationManager.NavigateTo("/Budget");
        }
    }
}
