using System.Security.Claims;

namespace FamilyBudget.Server.Services.Identity
{
    public class UserProvider : IUserProvider
    {
        public string UserId { get; private set; }

        public UserProvider(IHttpContextAccessor contextAccessor)
        {
            UserId = contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
