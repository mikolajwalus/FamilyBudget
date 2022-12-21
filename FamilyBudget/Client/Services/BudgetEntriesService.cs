using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.BudgetEntries;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text.Json;

namespace FamilyBudget.Client.Services
{
    public class BudgetEntriesService : IBudgetEntriesService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpInterceptorService _interceptor;

        public BudgetEntriesService(HttpClient httpClient, HttpInterceptorService interceptor)
        {
            _httpClient = httpClient;
            _interceptor = interceptor;
        }

        public async Task<BudgetEntryDto> CreateEntry(BudgetEntryForCreationDto dto)
        {
            _interceptor.MonitorEvent();

            var uri = BudgetEntriesApi.BudgetEntriesCreate;

            var result = await _httpClient.PostAsJsonAsync(uri, dto);

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await result.Content.ReadFromJsonAsync<BudgetEntryDto>();
        }

        public async Task DeleteEntry(Guid id)
        {
            _interceptor.MonitorEvent();

            var uri = BudgetEntriesApi.BudgetEntriesDelete.Replace(Common.IdRouteParam, id.ToString());

            await _httpClient.DeleteAsync(uri);
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }

        public async Task<BudgetEntriesDto> GetBudgetEntries(BudgetEntriesRequestDto dto)
        {
            _interceptor.MonitorEvent();

            var queryParams = new Dictionary<string, string>()
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(BudgetEntriesRequestDto.BudgetId))] = dto.BudgetId.ToString(),
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(BudgetEntriesRequestDto.CategoryId))] = dto.CategoryId.ToString(),
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(BudgetEntriesRequestDto.EntriesType))] = dto.EntriesType.ToString(),
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(BudgetEntriesRequestDto.PageNumber))] = dto.PageNumber.ToString(),
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(BudgetEntriesRequestDto.PageSize))] = dto.PageSize.ToString(),
            };

            var baseUri = BudgetEntriesApi.BudgetEntriesGetForBudget;

            var uri = QueryHelpers.AddQueryString(baseUri, queryParams);


            return await _httpClient.GetFromJsonAsync<BudgetEntriesDto>(uri);
        }

        public async Task<bool> UpdateEntry(BudgetEntryForUpdateDto dto)
        {
            _interceptor.MonitorEvent();

            var response = await _httpClient.PutAsJsonAsync(BudgetEntriesApi.BudgetEntriesUpdate, dto);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}
