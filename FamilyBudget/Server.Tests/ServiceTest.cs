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
        private SqliteConnection _connection;
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions = Options.Create(new OperationalStoreOptions());

        protected readonly string UserId = Guid.NewGuid().ToString();
        protected readonly IUserProvider UserProvider;

        protected ServiceTest()
        {
            UserProvider = Substitute.For<IUserProvider>();
        }

        public void Dispose()
        {
            _connection.Close();
        }

        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();

            using (var context = GetDbContext())
            {
                context.Database.EnsureCreated();

                var user = new Faker<ApplicationUser>()
                    .RuleFor(x => x.Id, UserId)
                    .Generate();

                context.Add(user);
                context.SaveChanges();
            }

            UserProvider.UserId.Returns(UserId);
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = GetDbContext())
            {
                context.Database.EnsureDeleted();
            }

            _connection.Close();
            _connection.Dispose();

            GC.Collect();
        }

        protected ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new ApplicationDbContext(options, _operationalStoreOptions);
        }

        protected async Task<ApplicationUser> GetMockedUser(ApplicationDbContext context) =>
            await context.Users.FindAsync(UserId);
    }
}
