namespace ImageGalleries.WebApi.Data
{
    public class Seeder
    {
        private readonly DataContext _dataContext;

        public Seeder(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task Seed()
        {
            //TODO

            _dataContext.Database.EnsureCreated();

            //await _dataContext.Users.AddRangeAsync(users);

            await _dataContext.SaveChangesAsync();
        }
    }
}
