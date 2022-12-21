namespace FamilyBudget.Shared.ApiRoutes
{
    public static class UsersApi
    {
        public const string UsersControllerRoute = Common.MainApiRoute + "/Users";

        public const string UsersAddUserAdminRole = UsersControllerRoute + "/AddAdmin/" + Common.IdRouteParam;
        public const string UsersRemoveUserAdminRole = UsersControllerRoute + "/RemoveAdmin/" + Common.IdRouteParam;
        public const string UsersGet = UsersControllerRoute + "/GetUsers";
    }
}
