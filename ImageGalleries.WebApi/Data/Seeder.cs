using ImageGalleries.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace ImageGalleries.WebApi.Data
{
    public class Seeder
    {
        private readonly IApplicationBuilder _applicationBuilder;
        private readonly IServiceScope _serviceScope;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _dataContext;

        public Seeder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            _serviceScope = _applicationBuilder.ApplicationServices.CreateScope();
            _roleManager = _serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _dataContext = _serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        }

        public Seeder(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public (User, List<User>) GetUsers()
        {
            var admin = new User()
            {
                Id = "admin",
                UserName = "Admin",
                Email = "admin@mail.com",
                EmailConfirmed = true,
                RegisterDate = DateTime.UtcNow,
                ProfilePictureUrl = "admin.jpg"

            };

            var users = new List<User>();
            var userInfos = new[]
            {
                ("white@mail.com", "Mr.White"),
                ("pink@mail.com", "Mr.Pink")
            };

            for (int i = 0; i < userInfos.Length; i++)
            {
                var userInfo = userInfos[i];

                var email = userInfo.Item1;
                var nickname = userInfo.Item2;

                var user = new User()
                {
                    Id = "user" + (i + 1),
                    UserName = nickname,
                    Email = email,
                    EmailConfirmed = true,
                    RegisterDate = DateTime.UtcNow,
                    ProfilePictureUrl = "user.jpg"
                };

                users.Add(user);
            }

            return (admin, users);
        }

        public async Task<(User, List<User>)> SeedRolesAndUsers()
        {
            if (!await _roleManager.RoleExistsAsync(Roles.AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.AdminRole));
            }

            if (!await _roleManager.RoleExistsAsync(Roles.UserRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.UserRole));
            }

            var password = "12345678";
            var users = GetUsers();
            var admin = users.Item1;
            var simpleUsers = users.Item2;

            if (await _userManager.FindByEmailAsync(admin.Email) == null)
            {
                await _userManager.CreateAsync(admin, password);
                await _userManager.AddToRoleAsync(admin, Roles.AdminRole);
            }

            for (int i = 0; i < simpleUsers.Count; i++)
            {
                if (await _userManager.FindByEmailAsync(simpleUsers[i].Email) == null)
                {
                    await _userManager.CreateAsync(simpleUsers[i], password);
                    await _userManager.AddToRoleAsync(simpleUsers[i], Roles.UserRole);
                }
            }

            return (admin, simpleUsers);
        }

        public async Task SeedData(User admin, List<User> users)
        {
            var random = new Random();

            var pictures = new List<Picture>();
            for (int i = 0; i < 5; i++)
            {
                var id = i + 1;

                pictures.Add(new Picture()
                {
                    Id = id.ToString(),
                    Url = $"Demo-image-{id}.jpg",
                    PreviewUrl = $"Demo-image-preview-{id}.jpg",
                    UploadTime = DateTime.UtcNow.AddHours(random.Next(-60, 20)),
                    Description = $"Description {id}",
                    UserId = users[i % users.Count].Id
                });
            }

            var galleries = new List<Gallery>();
            for (int i = 0; i < 5; i++)
            {
                var id = i + 1;

                galleries.Add(new Gallery()
                {
                    Id = id.ToString(),
                    Name = $"Gallery {id}",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20)),
                    Description = $"Gallery description {id}",
                    UserId = users[i % users.Count].Id
                });
            }

            var tags = new List<Tag>();
            for (int i = 0; i < 5; i++)
            {
                var id = i + 1;

                tags.Add(new Tag()
                {
                    Name = $"Tag {id}",
                    Description = $"This is tag {id}, also known as 'tagus {id}'",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                });
            }

            var pictureGalleries = new List<PictureGallery>()
            {
                new PictureGallery()
                {
                    PictureId = pictures[0].Id,
                    GalleryId = galleries[0].Id
                },

                new PictureGallery()
                {
                    PictureId = pictures[0].Id,
                    GalleryId = galleries[1].Id
                },

                new PictureGallery()
                {
                    PictureId = pictures[1].Id,
                    GalleryId = galleries[0].Id
                },

                new PictureGallery()
                {
                    PictureId = pictures[1].Id,
                    GalleryId = galleries[2].Id
                },

                new PictureGallery()
                {
                    PictureId = pictures[2].Id,
                    GalleryId = galleries[0].Id
                },
            };

            var pictureTags = new List<PictureTag>()
            {
                new PictureTag()
                {
                    PictureId = pictures[0].Id,
                    TagName = tags[0].Name
                },

                new PictureTag()
                {
                    PictureId = pictures[0].Id,
                    TagName = tags[1].Name
                },

                new PictureTag()
                {
                    PictureId = pictures[1].Id,
                    TagName = tags[0].Name
                },

                new PictureTag()
                {
                    PictureId = pictures[1].Id,
                    TagName = tags[2].Name
                },

                new PictureTag()
                {
                    PictureId = pictures[2].Id,
                    TagName = tags[0].Name
                },
            };

            var scores = new List<Score>()
            { 
                new Score()
                {
                    Amount = 1,
                    PictureId = pictures[0].Id,
                    UserId = users[0].Id,
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Score()
                {
                    Amount = 1,
                    PictureId = pictures[0].Id,
                    UserId = users[1].Id,
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Score()
                {
                    Amount = -1,
                    PictureId = pictures[1].Id,
                    UserId = users[0].Id,
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Score()
                {
                    Amount = 1,
                    PictureId = pictures[1].Id,
                    UserId = users[1].Id,
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Score()
                {
                    Amount = -1,
                    PictureId = pictures[3].Id,
                    UserId = users[1].Id,
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },
            };

            var comments = new List<Comment>()
            {
                new Comment()
                {
                    Id = "0",
                    PictureId = pictures[0].Id,
                    UserId = users[0].Id,
                    Content = "Comment number 1",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Comment()
                {
                    Id = "1",
                    PictureId = pictures[0].Id,
                    UserId = users[1].Id,
                    Content = "Comment number 2",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Comment()
                {
                    Id = "2",
                    PictureId = pictures[2].Id,
                    UserId = users[0].Id,
                    Content = "Comment number 3",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Comment()
                {
                    Id = "3",
                    PictureId = pictures[0].Id,
                    UserId = users[1].Id,
                    Content = "Comment number 4",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Comment()
                {
                    Id = "4",
                    PictureId = pictures[3].Id,
                    UserId = users[1].Id,
                    Content = "Comment number 5",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Comment()
                {
                    Id = "5",
                    PictureId = pictures[3].Id,
                    UserId = admin.Id,
                    Content = "Admin comment! number 6",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },

                new Comment()
                {
                    Id = "6",
                    PictureId = pictures[3].Id,
                    UserId = admin.Id,
                    Content = "Once again Admin comment! number 7",
                    CreationDate = DateTime.UtcNow.AddHours(random.Next(-60, 20))
                },
            };

            _dataContext.Database.EnsureCreated();
            await _dataContext.Pictures.AddRangeAsync(pictures);
            await _dataContext.Galleries.AddRangeAsync(galleries);
            await _dataContext.Tags.AddRangeAsync(tags);
            await _dataContext.PictureGalleries.AddRangeAsync(pictureGalleries);
            await _dataContext.PictureTags.AddRangeAsync(pictureTags);
            await _dataContext.Scores.AddRangeAsync(scores);
            await _dataContext.Comments.AddRangeAsync(comments);
            await _dataContext.SaveChangesAsync();
        }
    }
}
