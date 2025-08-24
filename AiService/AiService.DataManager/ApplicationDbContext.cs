using AiService.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AiService.DataManager
{
    public class ApplicationDbContext
    {
        public class ApplicationDBContext : DbContext
        {
            public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
            {

            }
            public DbSet<UserInfo> Categories { get; set; }
        }
    }
}
