using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.Budget;
using System.Net.Http.Json;

namespace FamilyBudget.Client.Services
{
    public class BudgetService : IBudgetService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpInterceptorService _interceptor;

        public BudgetService(HttpClient httpClient, HttpInterceptorService interceptor)
        {
            _httpClient = httpClient;
            _interceptor = interceptor;
        }

        public async Task<BudgetDto> CreateBudget(BudgetForCreationDto dto)
        {
            _interceptor.MonitorEvent();

            var result = await _httpClient.PostAsJsonAsync(BudgetApi.CreateBudget, dto);

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await result.Content.ReadFromJsonAsync<BudgetDto>();
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }

        public async Task<BudgetDto> GetBudget(Guid id)
        {
            _interceptor.MonitorEvent();

            var uri = BudgetApi.GetBudget.Replace(BudgetApi.BudgetIdRouteParam, id.ToString());

            return await _httpClient.GetFromJsonAsync<BudgetDto>(uri);
        }

        public async Task<List<BudgetDto>> GetUserBudgets()
        {
            _interceptor.MonitorEvent();

            return await _httpClient.GetFromJsonAsync<List<BudgetDto>>(BudgetApi.GetUserBudgets);
        }

        public async Task<List<UserForBudget>> GetUsers()
        {
            _interceptor.MonitorEvent();

            return await _httpClient.GetFromJsonAsync<List<UserForBudget>>(BudgetApi.GetUsers);
        }

        public async Task<List<UserForBudget>> GetUsersAssignedToBudget(Guid id)
        {
            _interceptor.MonitorEvent();

            var uri = BudgetApi.GetUsersAssignedToBudget.Replace(BudgetApi.BudgetIdRouteParam, id.ToString());

            return await _httpClient.GetFromJsonAsync<List<UserForBudget>>(uri);
        }

        public async Task<bool> UpdateBudget(BudgetForUpdateDto dto)
        {
            _interceptor.MonitorEvent();

            var response = await _httpClient.PutAsJsonAsync(BudgetApi.UpdateBudget, dto);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}
