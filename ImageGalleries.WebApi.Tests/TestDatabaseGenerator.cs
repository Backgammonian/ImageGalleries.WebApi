using ImageGalleries.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Tests
{
    public class TestDatabaseGenerator
    {
        public async Task<DataContext> GetDatabase()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dataContext = new DataContext(options);
            var seeder = new Seeder(dataContext);
            var users = seeder.GetUsers();
            dataContext.Users.Add(users.Item1);
            dataContext.Users.AddRange(users.Item2);
            await seeder.SeedData(users.Item1, users.Item2);

            return dataContext;
        }
    }
}
