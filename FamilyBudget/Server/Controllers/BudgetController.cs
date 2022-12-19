using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.Budget;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudget.Server.Controllers
{
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpPost(BudgetApi.AddUserToBudget)]
        public async Task<IActionResult> AddUserToBudget([FromRoute] string userId, [FromRoute] Guid budgetId)
        {
            await _budgetService.AddUserToBudget(userId, budgetId);

            return NoContent();
        }

        [HttpDelete(BudgetApi.RemoveUserFromBudget)]
        public async Task<IActionResult> RemoveUserFromBudget([FromRoute] string userId, [FromRoute] Guid budgetId)
        {
            await _budgetService.RemoveUserFromBudget(userId, budgetId);

            return NoContent();
        }

        [HttpGet(BudgetApi.GetBudget)]
        public async Task<BudgetDto> GetBudget([FromRoute] Guid budgetId)
        {
            return await _budgetService.GetBudget(budgetId);
        }

        [HttpPost(BudgetApi.CreateBudget)]
        public async Task<BudgetDto> CreateBudget([FromBody] BudgetForCreationDto dto)
        {
            return await _budgetService.CreateBudget(dto);
        }

        [HttpPut(BudgetApi.UpdateBudget)]
        public async Task<IActionResult> CreateBudget([FromBody] BudgetForUpdateDto dto)
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
        public async Task<List<UserAssignedToBudgetDto>> GetUsersAssignedBudget([FromRoute] Guid budgetId)
        {
            return await _budgetService.GetUsersAssignedToBudget(budgetId);
        }
    }
}
