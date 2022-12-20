using FamilyBudget.Server.Data;
using FamilyBudget.Server.Exceptions;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.Budget;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace FamilyBudget.Server.Services.Budgets
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _requestingUserId;

        public BudgetService(ApplicationDbContext context, IUserProvider userProvider)
        {
            _context = context;
            _requestingUserId = userProvider.UserId;
        }

        public async Task<List<UserForBudget>> GetUsers()
        {
            return await _context.Users.AsNoTracking()
                .Where(x => x.Id != _requestingUserId)
                .Select(x => new UserForBudget
                {
                    Id = x.Id,
                    Username = x.UserName,
                })
                .ToListAsync();
        }

        public async Task<List<UserForBudget>> GetUsersAssignedToBudget(Guid id)
        {
            var budget = await GetBudgetWithUsers(id);

            if (budget is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetBudgetNotExistsMessage(id));
            }

            if (!budget.UsersAssignedToBudget.Any(x => x.Id == _requestingUserId))
            {
                throw new UnauthorizedException(ResponseMessages.GetUserNotAssignedToBudgetMessage(id, _requestingUserId));
            }

            return budget.UsersAssignedToBudget
                .Select(x => new UserForBudget
                {
                    Id = x.Id,
                    Username = x.UserName
                })
                .ToList();
        }

        public async Task<BudgetDto> CreateBudget(BudgetForCreationDto dto)
        {
            ValidateBudgetName(dto.Name);

            await ValidateIfNameIsNotTaken(dto.Name);

            var requestedUsers = await _context.Users
                .Where(x => dto.AssignedUsers.Contains(x.Id) || x.Id == _requestingUserId)
                .ToListAsync();

            var usersNotExisting = dto.AssignedUsers.Where(x => !requestedUsers.Any(u => u.Id == x)).ToList();

            if (usersNotExisting.Count > 0)
            {
                var errors = usersNotExisting.Select(x => ResponseMessages.GetUserNotExistsMessage(x)).ToList();

                throw new BadRequestException(errors);
            }

            var budget = new Budget
            {
                Name = dto.Name,
                UsersAssignedToBudget = requestedUsers,
            };

            await _context.AddAsync(budget);
            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Balance = budget.Balance,
            };
        }

        public async Task UpdateBudget(BudgetForUpdateDto dto)
        {
            ValidateBudgetName(dto.Name);

            await ValidateIfNameIsNotTaken(dto.Name, dto.Id);

            var budget = await GetBudgetWithUsers(dto.Id);

            if (budget is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetBudgetNotExistsMessage(dto.Id));
            }

            if (!budget.UsersAssignedToBudget.Any(x => x.Id == _requestingUserId))
            {
                throw new UnauthorizedException(ResponseMessages.GetUserNotAssignedToBudgetMessage(dto.Id, _requestingUserId));
            }

            await PerformBudgetUpdate(dto, budget);
        }

        private async Task PerformBudgetUpdate(BudgetForUpdateDto dto, Budget budget)
        {
            await UpdateAssignedUsers(dto, budget);

            budget.Name = dto.Name;

            await _context.SaveChangesAsync();
        }

        public async Task<List<BudgetDto>> GetUserBudgets()
        {
            var user = await _context.Users
                .Include(x => x.UserBudgets)
                .Where(x => x.Id == _requestingUserId)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetUserNotExistsMessage(_requestingUserId));
            }

            return user.UserBudgets.Select(x => new BudgetDto()
            {
                Id = x.Id,
                Balance = x.Balance,
                Name = x.Name,
            })
            .ToList();
        }

        public async Task<BudgetDto> GetBudget(Guid id)
        {
            var budget = await _context.Budgets
                .Include(x => x.UsersAssignedToBudget)
                .Where(x => x.UsersAssignedToBudget.Any(user => user.Id == _requestingUserId) && x.Id == id)
                .FirstOrDefaultAsync();

            if (budget is null)
            {
                throw new ResourceNotFoundException(ResponseMessages.GetBudgetNotExistsMessage(id));
            }

            return new BudgetDto
            {
                Id = id,
                Balance = budget.Balance,
                Name = budget.Name,
                Users = budget.UsersAssignedToBudget.Where(x => x.Id != _requestingUserId).Select(x => new UserForBudget
                {
                    Id = x.Id,
                    Username = x.UserName
                })
                .ToList()
            };
        }

        private async Task UpdateAssignedUsers(BudgetForUpdateDto dto, Budget budget)
        {
            var users = await _context.Users
                .Where(x => dto.AssignedUsers.Contains(x.Id) || x.UserBudgets.Any(b => b.Id == dto.Id))
                .ToDictionaryAsync(x => x.Id);

            var usersToRemove = budget.UsersAssignedToBudget
                .Where(x => !dto.AssignedUsers.Contains(x.Id) && x.Id != _requestingUserId)
                .ToList();

            var usersToAdd = dto
                .AssignedUsers
                .Where(x => !budget.UsersAssignedToBudget.Any(u => u.Id == x))
                .Select(x => users[x])
                .ToList();

            foreach (var userToRemove in usersToRemove)
            {
                budget.UsersAssignedToBudget.Remove(userToRemove);
            }

            budget.UsersAssignedToBudget.AddRange(usersToAdd);
        }

        private async Task ValidateIfNameIsNotTaken(string name, Guid? budgetId = null)
        {
            var query = _context.Budgets.Where(x => x.Name == name);

            if (budgetId is not null)
            {
                query = query.Where(x => x.Id != budgetId.Value);
            }

            var budgetWithNameExists = await query.AnyAsync();

            if (budgetWithNameExists)
            {
                throw new BadRequestException(ResponseMessages.GetBudgetWithNameExistsMessage(name));
            }
        }

        private static void ValidateBudgetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BadRequestException(ResponseMessages.BudgetNameNotNullOrEmpty);
            }
        }

        private async Task<Budget> GetBudgetWithUsers(Guid budgetId)
        {
            return await _context.Budgets
                .Include(x => x.UsersAssignedToBudget)
                .FirstOrDefaultAsync(x => x.Id == budgetId);
        }
    }
}
