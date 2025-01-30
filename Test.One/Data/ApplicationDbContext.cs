using Microsoft.EntityFrameworkCore;
using Test.One.Models;

namespace Test.One.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<Users> Users { get; set; }

    }
}
