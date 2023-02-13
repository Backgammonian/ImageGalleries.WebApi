using ImageGalleries.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace ImageGalleries.WebApi.Data
{
    public class Seeder
    {
        private readonly IApplicationBuilder _applicationBuilder;

        public Seeder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
        }

        public async Task Seed()
        {
            #region Seeding roles and users
            using var serviceScope = _applicationBuilder.ApplicationServices.CreateScope();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(Roles.AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.AdminRole));
            }
            if (!await roleManager.RoleExistsAsync(Roles.UserRole))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.UserRole));
            }

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var adminEmail = "admin@mail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var newAdmin = new User()
                {
                    Id = "admin",
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    RegisterDate = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(newAdmin, "12345678");
                await userManager.AddToRoleAsync(newAdmin, Roles.AdminRole);

                admin = newAdmin;
            }

            var users = new List<User>();
            var userInfos = new[] 
            {
                ("white@mail.com", "Mr.White", "12345678"),
                ("pink@mail.com", "Mr.Pink", "12345678") 
            };

            for (int i = 0; i < userInfos.Length; i++)
            {
                var userInfo = userInfos[i];

                var email = userInfo.Item1;
                var nickname = userInfo.Item2;
                var password = userInfo.Item3;

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newUser = new User()
                    {
                        Id = "user" + (i + 1),
                        UserName = nickname,
                        Email = email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(newUser, password);
                    await userManager.AddToRoleAsync(newUser, Roles.UserRole);
                    users.Add(newUser);
                }
            }
            #endregion

            #region Seeding the data
            var dataContext = serviceScope.ServiceProvider.GetService<DataContext>();
            if (dataContext == null)
            {
                Console.WriteLine("(SeedData) Can't get the Data Context!");

                return;
            }

            //TODO

            dataContext.Database.EnsureCreated();
            //await _dataContext.Table.AddRangeAsync(items);
            await dataContext.SaveChangesAsync();
            #endregion
        }
    }
}
