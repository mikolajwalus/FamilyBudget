namespace FamilyBudget.Shared.ApiRoutes
{
    public static class BudgetEntryCategoriesApi
    {
        public const string BudgetControllerRoute = Common.MainApiRoute + "/BudgetCategories";

        public const string NameRouteParam = "{userId}";

        public const string BudgetCategoriesCreate = BudgetControllerRoute + "/" + NameRouteParam;
        public const string BudgetCategoriesUpdate = BudgetControllerRoute;
        public const string BudgetCategoriesDelete = BudgetControllerRoute + "/" + Common.IdRouteParam;
        public const string BudgetCategoriesGet = BudgetControllerRoute;
    }
}
