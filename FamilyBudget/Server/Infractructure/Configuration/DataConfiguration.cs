namespace FamilyBudget.Server.Infractructure.Configuration
{
    public class DataConfiguration
    {
        public const string SectionName = "DataConfiguration";
        /// <summary>
        /// Specifies if application will fill database with sample data 
        /// if no users in db
        /// </summary>
        public bool SeedData { get; set; }
        /// <summary>
        /// Removed all Identity Framework password 
        /// requrements
        /// Use for testing purposes
        /// </summary>
        public bool DisablePasswordRequirements { get; set; }
        /// <summary>
        /// Option to seed all users with same password
        /// specified as PasswordForSeededUsers
        /// </summary>
        public bool UseOnePassowordForSeededUsers { get; set; }
        /// <summary>
        /// Password used if
        /// UseOnePassowordForSeededUsers is true
        /// </summary>
        public string PasswordForSeededUsers { get; set; }
    }
}
