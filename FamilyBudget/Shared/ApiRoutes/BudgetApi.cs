namespace FamilyBudget.Shared.ApiRoutes
{
    public static class BudgetApi
    {
        public const string BudgetIdRouteParam = "{budgetId}";
        public const string UserIdRouteParam = "{userId}";

        public const string BudgetControllerRoute = Common.MainApiRoute + "/Budget";

        public const string CreateBudget = BudgetControllerRoute;
        public const string GetBudget = BudgetControllerRoute + "/" + BudgetIdRouteParam + "";
        public const string GetUsers = BudgetControllerRoute + "/Users";
        public const string GetUserBudgets = BudgetControllerRoute + "/UserBudgets";
        public const string GetUsersAssignedToBudget = BudgetControllerRoute + "/AssignedUsers/" + BudgetIdRouteParam;
        public const string UpdateBudget = BudgetControllerRoute;

    }
}
