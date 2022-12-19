using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.BudgetEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudget.Server.Controllers
{
    [Authorize]
    public class BudgetEntriesController : ControllerBase
    {
        private readonly IBudgetEntriesService _budgetEntriesService;

        public BudgetEntriesController(IBudgetEntriesService budgetEntriesService)
        {
            _budgetEntriesService = budgetEntriesService;
        }

        [HttpGet(BudgetEntriesApi.BudgetEntriesGetForBudget)]
        public async Task<BudgetEntriesDto> GetEntriesForBudget([FromQuery] BudgetEntriesRequestDto dto)
        {
            return await _budgetEntriesService.GetBudgetEntries(dto);
        }

        [HttpPost(BudgetEntriesApi.BudgetEntriesCreate)]
        public async Task<BudgetEntryDto> CreateEntry([FromBody] BudgetEntryForCreationDto dto)
        {
            return await _budgetEntriesService.CreateEntry(dto);
        }

        [HttpPut(BudgetEntriesApi.BudgetEntriesCreate)]
        public async Task<IActionResult> UpdateEntry([FromBody] BudgetEntryForUpdateDto dto)
        {
            await _budgetEntriesService.UpdateEntry(dto);

            return NoContent();
        }

        [HttpDelete(BudgetEntriesApi.BudgetEntriesDelete)]
        public async Task<IActionResult> DeleteEntry([FromQuery] Guid id)
        {
            await _budgetEntriesService.DeleteEntry(id);

            return NoContent();
        }
    }
}
