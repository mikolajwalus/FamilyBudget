namespace FamilyBudget.Server.Exceptions
{
    public class BadRequestException : Exception
    {
        public readonly List<string> Errors;
        public BadRequestException(List<string> errors)
        {
            Errors = errors;
        }

        public BadRequestException(string error)
        {
            Errors = new List<string> { error };
        }
    }
}
