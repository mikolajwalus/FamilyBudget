namespace FamilyBudget.Shared.ApiRoutes
{
    public static class BudgetEntriesApi
    {
        public const string BudgetControllerRoute = Common.MainApiRoute + "BudgetEntries";
        public const string BudgetEntriesCreate = BudgetControllerRoute;
        public const string BudgetEntriesUpdate = BudgetControllerRoute;
        public const string BudgetEntriesDelete = BudgetControllerRoute;
        public const string BudgetEntriesGetForBudget = BudgetControllerRoute + "/GetForBudget";
    }
}
