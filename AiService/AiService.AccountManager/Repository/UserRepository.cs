using AiService.AccountManager.Repository.Interfaces;
using AiService.DataManager;
using AiService.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiService.AccountManager.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db) => _db = db;

        public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task AddAsync(ApplicationUser user, CancellationToken ct = default)
            => await _db.Users.AddAsync(user, ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => await _db.Users.AnyAsync(u => u.Email == email, ct);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
