using Bogus;
using Duende.IdentityServer.EntityFramework.Options;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace FamilyBudget.Server.Tests
{
    public class ServiceTest
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;

        protected readonly string UserId = Guid.NewGuid().ToString();
        protected readonly IUserProvider UserProvider;
        protected readonly ApplicationDbContext DbContext;

        protected ServiceTest()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(_connection)
                    .Options;

            var operationalStoreOptions = Options.Create(new OperationalStoreOptions());

            DbContext = new ApplicationDbContext(options, operationalStoreOptions);

            UserProvider = Substitute.For<IUserProvider>();
            UserProvider.UserId.Returns(UserId);
        }

        public void Dispose()
        {
            _connection.Close();
        }

        [SetUp]
        public void SetUp()
        {
            DbContext.Database.EnsureCreated();

            var user = new Faker<ApplicationUser>()
                .RuleFor(x => x.Id, UserId)
                .Generate();

            DbContext.Add(user);
            DbContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            DbContext.Database.EnsureDeletedAsync();
        }

        protected async Task<ApplicationUser> GetMockedUser() =>
            await DbContext.Users.FindAsync(UserId);
    }
}
