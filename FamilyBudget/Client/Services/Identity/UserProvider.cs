using Microsoft.AspNetCore.Components.Authorization;

namespace FamilyBudget.Client.Services
{
    public class UserProvider : IUserProvider
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public UserProvider(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> GetUserId()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                return user.FindFirst(c => c.Type == "sub").Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
