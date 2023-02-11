using ImageGalleries.WebApi.Data;
using ImageGalleries.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Repositories.RefreshTokens
{
    public class DatabaseRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _context;

        public DatabaseRefreshTokenRepository(DataContext context)
        {
            _context = context;
        }

        public async Task Create(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var refreshToken = await _context.RefreshTokens.FindAsync(id);
            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAll(string userId)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByToken(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
