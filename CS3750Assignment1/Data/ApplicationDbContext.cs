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
        public DbSet<Course> Courses { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<Submission> Submissions { get; set; }


        /// <summary>
        /// ensures that the entity framework is aware of the foreign key instructorId 
        /// in course that references User
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany()
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Course)
                .WithMany()
                .HasForeignKey(r => r.CourseId);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Instructor)
                .WithMany()
                .HasForeignKey(a => a.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<CS3750Assignment1.Models.Assignment> Assignment { get; set; } = default!;
    }

}
