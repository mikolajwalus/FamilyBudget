using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.Budget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudget.Server.Controllers
{
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet(BudgetApi.GetBudget)]
        public async Task<BudgetDto> GetBudget([FromRoute] Guid budgetId)
        {
            return await _budgetService.GetBudget(budgetId);
        }

        [HttpGet(BudgetApi.GetUsers)]
        public async Task<List<UserForBudget>> GetUsers()
        {
            return await _budgetService.GetUsers();
        }

        [HttpPost(BudgetApi.CreateBudget)]
        public async Task<BudgetDto> CreateBudget([FromBody] BudgetForCreationDto dto)
        {
            return await _budgetService.CreateBudget(dto);
        }

        [HttpPut(BudgetApi.UpdateBudget)]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetForUpdateDto dto)
        {
            await _budgetService.UpdateBudget(dto);

            return NoContent();
        }

        [HttpGet(BudgetApi.GetUserBudgets)]
        public async Task<List<BudgetDto>> GetUserBudgets()
        {
            return await _budgetService.GetUserBudgets();
        }

        [HttpGet(BudgetApi.GetUsersAssignedToBudget)]
        public async Task<List<UserForBudget>> GetUsersAssignedBudget([FromRoute] Guid budgetId)
        {
            return await _budgetService.GetUsersAssignedToBudget(budgetId);
        }
    }
}
