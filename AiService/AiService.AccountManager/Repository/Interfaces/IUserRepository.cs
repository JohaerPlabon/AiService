using AiService.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiService.AccountManager.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(ApplicationUser user, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
