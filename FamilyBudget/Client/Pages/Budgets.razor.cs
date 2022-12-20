using FamilyBudget.Client.Services;
using FamilyBudget.Shared.Budget;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FamilyBudget.Client.Pages
{
    public partial class Budgets : ComponentBase
    {
        [Inject]
        IBudgetService BudgetService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        NotificationService NotificationService { get; set; }

        public List<BudgetDto> UserBudgets;

        protected override async Task OnInitializedAsync()
        {
            UserBudgets = await BudgetService.GetUserBudgets();
        }

        void Edit(Guid id)
        {
            NavigationManager.NavigateTo($"/Budget/{id}");
        }

        void Add()
        {
            NavigationManager.NavigateTo("/Budget");
        }
    }
}
