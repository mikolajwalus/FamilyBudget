namespace FamilyBudget.Server.Exceptions
{
    public class UserNotExistException : Exception
    {
        public const string MessageTemplate = "User with Id {0} doesn't exist";

        public UserNotExistException(string id) : base(String.Format(MessageTemplate, id))
        {
        }
    }
}
