using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }

}
