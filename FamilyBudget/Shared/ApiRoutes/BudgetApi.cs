namespace FamilyBudget.Shared.ApiRoutes
{
    public static class BudgetApi
    {
        public const string BudgetControllerRoute = Common.MainApiRoute + "Budget";

        public const string AddUserToBudget = BudgetControllerRoute + "/User/{userId}/{budgetId}";
        public const string CreateBudget = BudgetControllerRoute;
        public const string GetBudget = BudgetControllerRoute + "/{budgetId}";
        public const string GetUserBudgets = BudgetControllerRoute + "/UserBudgets";
        public const string GetUsersAssignedToBudget = BudgetControllerRoute + "/AssignedUsers/{budgetId}";
        public const string RemoveUserFromBudget = AddUserToBudget;
        public const string UpdateBudget = BudgetControllerRoute;
    }
}
