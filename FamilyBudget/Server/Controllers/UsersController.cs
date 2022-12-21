using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.ApiRoutes;
using FamilyBudget.Shared.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudget.Server.Controllers
{
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet(UsersApi.UsersGet)]
        public async Task<List<UserForAdminConsoleDto>> Get()
        {
            return await _userService.GetUsers();
        }


        [HttpPost(UsersApi.UsersAddUserAdminRole)]
        public async Task<IActionResult> AddUserAdminRole(string id)
        {
            await _userService.AddUserAdminRole(id);

            return NoContent();
        }

        [HttpPost(UsersApi.UsersRemoveUserAdminRole)]
        public async Task<IActionResult> RemoveUserAdminRole(string id)
        {
            await _userService.RemoveUserAdminRole(id);

            return NoContent();
        }
    }
}
