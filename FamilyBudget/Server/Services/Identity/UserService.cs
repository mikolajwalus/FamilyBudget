using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Shared.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly IUserProvider _userProvider;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserProvider userProvider, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userProvider = userProvider;
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<UserForAdminConsoleDto>> GetUsers()
        {
            var roleQuery = _context.Roles
                .Where(x => x.Name == Roles.Admin)
                .Select(x => x.Id);

            var adminIds = await _context
                .UserRoles
                .Where(x => roleQuery.Contains(x.RoleId))
                .Select(x => x.UserId)
                .Distinct()
                .ToDictionaryAsync(x => x);


            var dtos = await _context.Users
                .Select(x => new UserForAdminConsoleDto
                {
                    Id = x.Id,
                    Username = x.UserName,

                })
                .ToListAsync();

            foreach (var dto in dtos)
            {
                if (adminIds.ContainsKey(dto.Id))
                {
                    dto.IsAdmin = true;
                }
            }

            return dtos;
        }

        public async Task AddUserAdminRole(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetUserNotExistsMessage(id));
            }

            await _userManager.AddToRoleAsync(user, Roles.Admin);
        }

        public async Task RemoveUserAdminRole(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (id == _userProvider.UserId)
            {
                throw new BadRequestException(ResponseMessages.UserRemovingHimselfFromAdmin);
            }

            if (user is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetUserNotExistsMessage(id));
            }

            await _userManager.RemoveFromRoleAsync(user, Roles.Admin);
        }
    }
}
