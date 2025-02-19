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
    }
}
