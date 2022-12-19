namespace FamilyBudget.Server.Exceptions
{
    public class BudgetNotExistException : Exception
    {
        public const string MessageTemplate = "Budget with Id {0} doesn't exist";

        public BudgetNotExistException(Guid id) : base(String.Format(MessageTemplate, id))
        {
        }
    }
}
