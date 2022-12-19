namespace FamilyBudget.Server.Exceptions
{
    public class UserNotAssignedToBudgetException : Exception
    {
        public const string MessageTemplate = "User with Id {0} is not assignedto budget ith Id {1}";

        public UserNotAssignedToBudgetException(string userId, string budgetId)
            : base(String.Format(MessageTemplate, userId, budgetId))
        {
        }
    }
}
