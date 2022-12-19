﻿namespace FamilyBudget.Server.Exceptions
{
    public static class ResponseMessages
    {
        public const string BudgetNotFound = "Budget with Id {0} doesn't exist";
        public const string BudgetWithNameAleardyExists = "Budget with Name {0} already exist";
        public const string UserNotAssignedToBudget = "User with Id {0} is not assigned to budget with Id {1}";
        public const string UserNotExist = "User with Id {0} doesn't exist";
        public const string UserRemovingHimselfFromBudget = "User cannot remove himself from budget";
        public const string BudgetNameNotNullOrEmpty = "Budget name ecannot be empty, or contains only whitespaces";

        public static string GetBudgetNotExistsMessage(Guid budgetId)
        {
            return string.Format(BudgetNotFound, budgetId);
        }

        public static string GetBudgetWithNameExistsMessage(string name)
        {
            return string.Format(BudgetWithNameAleardyExists, name);
        }

        public static string GetGetUserNotAssignedToBudgetMessage(Guid budgetId, string userId)
        {
            return string.Format(UserNotAssignedToBudget, userId, budgetId);
        }

        public static string GetUserNotExistsMessage(string userId)
        {
            return string.Format(UserNotExist, userId);
        }
    }
}