using AiService.Models;
using Microsoft.EntityFrameworkCore;

namespace AiService.DataManager
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Categories { get; set; }
    }
}
