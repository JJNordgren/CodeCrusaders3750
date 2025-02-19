using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Collections.Generic;

namespace CS3750Assignment1Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("InMemoryDb");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data

                int N = db.Courses.Count();

                Course course = new Course
                {
                    Department = "CS",
                    Name = "1010",
                    ClassNumber = "1010",
                    CreditHours = 3,
                    Capacity = 30,
                    Location = "NB 101",
                    StartTime = DateTime.Parse("08:00 AM").TimeOfDay,
                    EndTime = DateTime.Parse("09:00 PM").TimeOfDay,
                    InstructorId = 17, // WaldoWildcat@weber.edu
                    MeetingDays = new List<string> { "Monday", "Wednesday", "Friday" }
                };

                db.Courses.Add(course);

                // Saves it to the database
                db.SaveChanges();

                int M = db.Courses.Count();

                // Assert: Verify expected results
                Assert.IsTrue(M == N + 1);
            }
        }

        [TestMethod]
        public async Task StudentRegisterCourse()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()); // Use unique database name
            //.UseInMemoryDatabase("InMemoryDb");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                int N = db.Registrations.Count();

                var userId = 19; // Greg Heffley
                var courseId = 11; // Historic Analysis, History 3332

                var course = await db.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                //var course = new Course { };
                var student = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                //var student = new User { };
                // Id
                //email 
                //firstName
                //lastName 
                //[Required]
                //[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
                //password
                //[Required]
                //[MinAge(16, ErrorMessage = "You must be at least 16 years old.")]
                // BirthDate (DateTime) 


                var registration = new Registration
                {
                    UserId = userId,
                    Student = student,
                    CourseId = courseId,
                    Course = course
                };

                db.Registrations.Add(registration);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Error saving registration: {ex.Message}");
                }


                int M = db.Registrations.Count();

                // Assert: Verify expected results
                Assert.IsTrue(M == N + 1);
            }
        }
    }
}
