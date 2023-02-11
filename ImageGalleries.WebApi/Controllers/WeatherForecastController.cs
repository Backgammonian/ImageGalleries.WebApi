using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherForecastController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching"
        };

        private readonly IRandomGenerator _randomGenerator;
        private readonly UserManager<User> _userRepository;

        public WeatherForecastController(IRandomGenerator randomGenerator,
            UserManager<User> userRepository)
        {
            _randomGenerator = randomGenerator;
            _userRepository = userRepository;
        }

        private IEnumerable<Forecast> GetForecast(int count)
        {
            return Enumerable.Range(1, count).Select(index => new Forecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = _randomGenerator.Next(-100, 100),
                Summary = Summaries[_randomGenerator.Next(Summaries.Length)]
            });
        }

        [HttpGet("public")]
        public IActionResult GetPublicForecast()
        {
            return Ok(GetForecast(1));
        }

        [Authorize]
        [HttpGet("authorized-only")]
        public async Task<IActionResult> GetAuthorizedOnlyForecast()
        {
            var userId = HttpContext.User.FindFirstValue("UserId") ?? string.Empty;
            var user = await _userRepository.FindByIdAsync(userId);
            if (user != null) 
            {
                return Ok(new
                {
                    Forecast = GetForecast(2),
                    User = user
                });
            }

            return Ok(GetForecast(2));
        }

        [Authorize(Roles = Roles.UserRole)]
        [HttpGet("users-only")]
        public IActionResult GetUsersOnlyForecast()
        {
            return Ok(GetForecast(3));
        }

        [Authorize(Roles = Roles.AdminRole)]
        [HttpGet("admins-only")]
        public IActionResult GetAdminsOnlyForecast()
        {
            return Ok(GetForecast(4));
        }

        [Authorize(Roles = $"{Roles.UserRole}, {Roles.AdminRole}")]
        [HttpGet("user-and-admin-roles")]
        public IActionResult GetUserAndAdminRolesForecast()
        {
            return Ok(GetForecast(5));
        }
    }

    public class Forecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string Summary { get; set; } = string.Empty;
    }
}
