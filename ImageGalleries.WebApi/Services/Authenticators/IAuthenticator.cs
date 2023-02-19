using ImageGalleries.WebApi.Models;
using ImageGalleries.WebApi.Responses;

namespace ImageGalleries.WebApi.Services.Authenticators
{
    public interface IAuthenticator
    {
        Task<AuthenticatedUserResponse> Authenticate(User user, string? role);
    }
}
