using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.Users;
using System.Net.Http.Json;

namespace FamilyBudget.Client.Services.Identity
{
    public class UserService : IUserService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpInterceptorService _interceptor;

        public UserService(HttpClient httpClient, HttpInterceptorService interceptor)
        {
            _httpClient = httpClient;
            _interceptor = interceptor;
        }

        public async Task AddUserAdminRole(string id)
        {
            _interceptor.MonitorEvent();

            var uri = UsersApi.UsersAddUserAdminRole.Replace(Common.IdRouteParam, id.ToString());

            await _httpClient.PostAsync(uri, null);
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }

        public async Task<List<UserForAdminConsoleDto>> GetUsers()
        {
            _interceptor.MonitorEvent();

            return await _httpClient.GetFromJsonAsync<List<UserForAdminConsoleDto>>(UsersApi.UsersGet);
        }

        public async Task RemoveUserAdminRole(string id)
        {
            _interceptor.MonitorEvent();

            var uri = UsersApi.UsersRemoveUserAdminRole.Replace(Common.IdRouteParam, id.ToString());

            await _httpClient.PostAsync(uri, null);
        }
    }
}
