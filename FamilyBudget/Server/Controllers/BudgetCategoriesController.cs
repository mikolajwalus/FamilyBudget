using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.BudgetEntries;
using FamilyBudget.Shared.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudget.Server.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class BudgetEntryCategoriesController : ControllerBase
    {
        private readonly IBudgetEntryCategoriesService _budgetEntriesService;

        public BudgetEntryCategoriesController(IBudgetEntryCategoriesService budgetEntriesService)
        {
            _budgetEntriesService = budgetEntriesService;
        }

        [HttpGet(BudgetEntryCategoriesApi.BudgetCategoriesGet)]
        public async Task<List<BudgetEntryCategoryDto>> Get()
        {
            return await _budgetEntriesService.GetAll();
        }

        [HttpPost(BudgetEntryCategoriesApi.BudgetCategoriesCreate)]
        public async Task<BudgetEntryCategoryDto> CreateEntry([FromRoute] string name)
        {
            return await _budgetEntriesService.Create(name);
        }

        [HttpPut(BudgetEntryCategoriesApi.BudgetCategoriesUpdate)]
        public async Task<IActionResult> UpdateEntry([FromBody] BudgetEntryCategoryDto dto)
        {
            await _budgetEntriesService.Update(dto);

            return NoContent();
        }

        [HttpDelete(BudgetEntryCategoriesApi.BudgetCategoriesDelete)]
        public async Task<IActionResult> DeleteEntry([FromQuery] Guid id)
        {
            await _budgetEntriesService.Delete(id);

            return NoContent();
        }
    }
}
