using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByToken(string token);
        Task Create(RefreshToken refreshToken);
        Task Delete(string id);
        Task DeleteAll(string userId);
    }
}