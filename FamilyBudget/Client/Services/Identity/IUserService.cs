using FamilyBudget.Shared.Users;

namespace FamilyBudget.Client.Services.Identity
{
    public interface IUserService
    {
        Task AddUserAdminRole(string id);
        Task<List<UserForAdminConsoleDto>> GetUsers();
        Task RemoveUserAdminRole(string id);
    }
}
