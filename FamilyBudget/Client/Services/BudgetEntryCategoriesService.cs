using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.BudgetEntries;
using System.Net.Http.Json;

namespace FamilyBudget.Client.Services
{
    public class BudgetEntryCategoriesService : IBudgetEntryCategoriesService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpInterceptorService _interceptor;

        public BudgetEntryCategoriesService(HttpClient httpClient, HttpInterceptorService interceptor)
        {
            _httpClient = httpClient;
            _interceptor = interceptor;
        }

        public async Task<BudgetEntryCategoryDto> Create(string name)
        {
            _interceptor.MonitorEvent();

            var uri = BudgetEntryCategoriesApi.BudgetCategoriesCreate.Replace(BudgetEntryCategoriesApi.NameRouteParam, name);

            var result = await _httpClient.PostAsync(uri, null);

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await result.Content.ReadFromJsonAsync<BudgetEntryCategoryDto>();
        }

        public async Task Delete(Guid id)
        {
            _interceptor.MonitorEvent();

            var uri = BudgetEntryCategoriesApi.BudgetCategoriesDelete.Replace(Common.IdRouteParam, id.ToString());

            await _httpClient.DeleteAsync(uri);
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }

        public async Task<List<BudgetEntryCategoryDto>> GetAll()
        {
            _interceptor.MonitorEvent();

            var uri = BudgetEntryCategoriesApi.BudgetCategoriesGet;

            return await _httpClient.GetFromJsonAsync<List<BudgetEntryCategoryDto>>(uri);
        }

        public async Task<BudgetEntryCategoryDto> Update(BudgetEntryCategoryDto dto)
        {
            _interceptor.MonitorEvent();

            var response = await _httpClient.PutAsJsonAsync(BudgetEntryCategoriesApi.BudgetCategoriesUpdate, dto);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return dto;
        }
    }
}
