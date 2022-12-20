using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.Budget;
using System.Net.Http.Json;

namespace FamilyBudget.Client.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly HttpClient _httpClient;

        public BudgetService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BudgetDto> CreateBudget(BudgetForCreationDto dto)
        {
            var result = await _httpClient.PostAsJsonAsync(BudgetApi.CreateBudget, dto);

            return await result.Content.ReadFromJsonAsync<BudgetDto>();
        }

        public async Task<BudgetDto> GetBudget(Guid id)
        {
            var uri = BudgetApi.GetBudget.Replace(BudgetApi.BudgetIdRouteParam, id.ToString());

            return await _httpClient.GetFromJsonAsync<BudgetDto>(uri);
        }

        public async Task<List<BudgetDto>> GetUserBudgets()
        {
            return await _httpClient.GetFromJsonAsync<List<BudgetDto>>(BudgetApi.GetUserBudgets);
        }

        public async Task<List<UserForBudget>> GetUsers()
        {
            return await _httpClient.GetFromJsonAsync<List<UserForBudget>>(BudgetApi.GetUsers);
        }

        public async Task<List<UserForBudget>> GetUsersAssignedToBudget(Guid id)
        {
            var uri = BudgetApi.GetUsersAssignedToBudget.Replace(BudgetApi.BudgetIdRouteParam, id.ToString());

            return await _httpClient.GetFromJsonAsync<List<UserForBudget>>(uri);
        }

        public async Task UpdateBudget(BudgetForUpdateDto dto)
        {
            await _httpClient.PutAsJsonAsync(BudgetApi.UpdateBudget, dto);
        }
    }
}
