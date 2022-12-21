namespace FamilyBudget.Server.Exceptions
{
    public static class ResponseMessages
    {
        public const string BudgetNotFound = "Budget with Id {0} doesn't exist";
        public const string BudgetWithNameAleardyExists = "Budget with Name {0} already exist";
        public const string BudgetNameNotNullOrEmpty = "Budget name ecannot be empty, or contains only whitespaces";
        public const string BudgetEntryCategoryWithNameAleardyExists = "Budget entry category with Name {0} already exist";
        public const string BudgetEntryNotFound = "Budget entry with Id {0} doesn't exist";
        public const string BudgetEntryMoneyNotZero = "Budget entry money amount cannot be zero";
        public const string CategoryNotFound = "Budget entry category with Id {0} doesn't exist";
        public const string UserNotAssignedToBudget = "User with Id {0} is not assigned to budget with Id {1}";
        public const string UserNotExist = "User with Id {0} doesn't exist";
        public const string UserRemovingHimselfFromBudget = "User cannot remove himself from budget";

        public static string GetBudgetNotExistsMessage(Guid budgetId)
        {
            return string.Format(BudgetNotFound, budgetId);
        }

        public static string GetBudgetEntryNotExistsMessage(Guid budgetEntryId)
        {
            return string.Format(BudgetEntryNotFound, budgetEntryId);
        }

        public static string GetCategoryNotExistsMessage(Guid categoryId)
        {
            return string.Format(CategoryNotFound, categoryId);
        }

        public static string GetBudgetWithNameExistsMessage(string name)
        {
            return string.Format(BudgetWithNameAleardyExists, name);
        }

        public static string GetBudgetEntryCategoryWithNameExistsMessage(string name)
        {
            return string.Format(BudgetEntryCategoryWithNameAleardyExists, name);
        }

        public static string GetUserNotAssignedToBudgetMessage(Guid budgetId, string userId)
        {
            return string.Format(UserNotAssignedToBudget, userId, budgetId);
        }

        public static string GetUserNotExistsMessage(string userId)
        {
            return string.Format(UserNotExist, userId);
        }
    }
}
