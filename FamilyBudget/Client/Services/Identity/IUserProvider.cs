namespace FamilyBudget.Client.Services
{
    public interface IUserProvider
    {
        Task<string> GetUserId();
    }
}