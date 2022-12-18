namespace FamilyBudget.Server.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
