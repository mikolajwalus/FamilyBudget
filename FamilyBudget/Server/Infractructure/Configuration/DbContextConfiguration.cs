namespace FamilyBudget.Server.Infractructure.Configuration
{
    public class DbContextConfiguration
    {
        public const string SectionName = "ContextOptions";
        /// <summary>
        /// Functionallity to automaticly generate
        /// timestamps on creating and updateing entities
        /// should always be false
        /// can be true only for testing purposes
        /// </summary>
        public bool TurnOffUpdatingTimestamps { get; set; }
    }
}
