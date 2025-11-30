using Microsoft.EntityFrameworkCore;

namespace ErpCrm.Infrastructure.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {}
        public DbSet<User> Users { get; set; }
        // Additional DbSets for other entities
    }
}