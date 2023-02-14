using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Repositories.Galleries;
using ImageGalleries.WebApi.Repositories.Pictures;
using ImageGalleries.WebApi.Repositories.RefreshTokens;
using ImageGalleries.WebApi.Repositories.Tags;
using ImageGalleries.WebApi.Repositories.Users;
using ImageGalleries.WebApi.Services.Authenticators;
using ImageGalleries.WebApi.Services.PhotoServices;
using ImageGalleries.WebApi.Services.RandomGenerators;
using ImageGalleries.WebApi.Services.TokenGenerators;
using ImageGalleries.WebApi.Services.TokenValidators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace ImageGalleries.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });

            builder.Services.AddIdentityCore<User>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.,-()";
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequiredLength = 0;
                o.Password.RequiredUniqueChars = 0;
            }).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            var authenticationConfiguration = new AuthenticationConfiguration();
            builder.Configuration.Bind("Authentication", authenticationConfiguration);
            builder.Services.AddSingleton(authenticationConfiguration);

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            builder.Services.AddDbContext<DataContext>(o =>
            {
                o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSingleton<AccessTokenGenerator>();
            builder.Services.AddSingleton<RefreshTokenGenerator>();
            builder.Services.AddSingleton<RefreshTokenValidator>();
            builder.Services.AddScoped<Authenticator>();
            builder.Services.AddSingleton<TokenGenerator>();
            builder.Services.AddScoped<IRefreshTokenRepository, DatabaseRefreshTokenRepository>();
            builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();          
            builder.Services.AddScoped<IPictureRepository, PictureRepository>();
            builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
                    ValidIssuer = authenticationConfiguration.Issuer,
                    ValidAudience = authenticationConfiguration.Audience,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            var webApiName = "Image Galleries WebAPI";
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = webApiName,
                    Description = webApiName
                });

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });

                o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            if (args.Length > 0 &&
                args[0].ToLower() == "SeedData".ToLower())
            {
                await SeedData(app);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(o =>
                {
                    o.SwaggerEndpoint("/swagger/V1/swagger.json", webApiName);
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static async Task SeedData(IApplicationBuilder app)
        {
            Console.WriteLine("(SeedData) Seeding the database");

            var seeder = new Seeder(app);
            await seeder.Seed();
        }
    }
}