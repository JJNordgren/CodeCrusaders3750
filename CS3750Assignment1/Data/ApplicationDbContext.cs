using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        public DbSet<Assignment> Assignment { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        //public DbSet<Notification> Notifications { get; set; }


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

            //the database can't recognize a dictionary, so I have to convert it to json and deserialize it when you call it.
            var notificationConverter = new ValueConverter<Dictionary<string, string>?, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null));

            //makes sure it updates correctly using a value comparer
            var notificationComparer = new ValueComparer<Dictionary<string, string>?>(
                (d1, d2) =>
                    d1 == null && d2 == null ||
                    d1 != null && d2 != null && d1.Count == d2.Count && !d1.Except(d2).Any(),
                d => d == null ? 0 : d.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                d => d == null ? null : new Dictionary<string, string>(d)
            );

            modelBuilder.Entity<User>()
                .Property(u => u.notification)
                .HasConversion(notificationConverter)
                .Metadata.SetValueComparer(notificationComparer);
        }
        //public DbSet<CS3750Assignment1.Models.Assignment> Assignment { get; set; } = default!;
    }

}
