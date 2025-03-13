using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using CS3750Assignment1.Pages;


namespace CS3750Assignment1Test
{
    [TestClass]
    public class UnitTests
    {

        /*

        Implementation below:
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");


        Professor Christi Instructions:
        DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
        DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
        SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;", null);
        var _context = new ApplicationDbContext((DbContextOptions<ApplicationDbContext>)builder.Options);

         */

        // I'm pretty sure this is what the professor's above code is referring to (also going off of the razor page unit test documentation)
        // So with a particular test I can just call `using(var _context = new ApplicationDbContext(TestDbContextOptions()))`  -Matt
        public static DbContextOptions<ApplicationDbContext> TestDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;")
                .Options;
        }

        [TestMethod]
        public async Task UpdateUserProfile()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                var userId = 106; // Use a valid user ID
                User user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                bool hadToUseADummyUser = false;
                if (user == null)
                {
                    hadToUseADummyUser = true;

                    user = new User()
                    {
                        firstName = "Test",
                        lastName = "User",
                        email = "test@user.com",
                        password = "password",
                        confirmPassword = "password",
                        BirthDate = DateTime.Parse("01-01-1990"),
                        userType = 'S',
                        addressLine1 = "Old Address 1",
                        addressLine2 = "Old Address 2",
                        city = "Old City",
                        state = "Old State",
                        zip = "00000",
                        phone = "000-000-0000"
                    };

                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }

                // Act: Update the user's address
                user.addressLine1 = "New Address 1";
                user.addressLine2 = "New Address 2";
                user.city = "New City";
                user.state = "New State";
                user.zip = "12345";
                user.phone = "123-456-7890";

                db.Users.Update(user);
                await db.SaveChangesAsync();

                // Assert: Verify that the address was updated
                var updatedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                Assert.IsNotNull(updatedUser);
                Assert.AreEqual("New Address 1", updatedUser.addressLine1);
                Assert.AreEqual("New Address 2", updatedUser.addressLine2);
                Assert.AreEqual("New City", updatedUser.city);
                Assert.AreEqual("New State", updatedUser.state);
                Assert.AreEqual("12345", updatedUser.zip);
                Assert.AreEqual("123-456-7890", updatedUser.phone);

                // Clean up: Remove the test data
                if (hadToUseADummyUser)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                }
            }
        }



        [TestMethod]
        public async Task UpdateAmountPaidAsync_UpdatesCorrectly()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Create test user or retrieve existing one
                var userId = 106; // Use a valid user ID
                User user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                bool hadToUseDummyUser = false;
                if (user == null)
                {
                    hadToUseDummyUser = true;
                    user = new User
                    {
                        firstName = "Test",
                        lastName = "User",
                        email = "test@user.com",
                        password = "password",
                        confirmPassword = "password",
                        BirthDate = DateTime.Parse("01-01-1990"),
                        userType = 'S',
                        addressLine1 = "Address 1",
                        amountPaid = 0 // Initial amount
                    };

                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                }

                long paymentAmount = 500;

                // Act: Call the method to update the payment
                user.amountPaid += paymentAmount;
                db.Users.Update(user);
                await db.SaveChangesAsync();

                // Assert: Verify the update
                var updatedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                Assert.IsNotNull(updatedUser);
                Assert.AreEqual(500, updatedUser.amountPaid);

                // Clean up: Remove test data if it was created
                if (hadToUseDummyUser)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                }
            }
        }
        [TestMethod]
        public async Task ViewClassDetailsTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data

                // Ensure the instructor exists
                var instructorId = 1;
                var instructor = await db.Users.FirstOrDefaultAsync(u => u.Id == instructorId);
                if (instructor == null)
                {
                    instructor = new User
                    {
                        firstName = "John",
                        lastName = "Doe",
                        email = "johndoe@weber.edu",
                        password = "password123",
                        confirmPassword = "password123",
                        BirthDate = DateTime.Parse("01-01-1980"),
                        userType = 'T'
                    };
                    db.Users.Add(instructor);
                    await db.SaveChangesAsync();
                    instructorId = instructor.Id; // Update instructorId with the newly created instructor's Id
                }

                var course = new Course
                {
                    Department = "CS",
                    Name = "Software Engineering II",
                    ClassNumber = "3750",
                    CreditHours = 4,
                    Capacity = 35,
                    Location = "Noorda 318",
                    MeetingDays = new List<string> { "Monday", "Wednesday" },
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 30, 0),
                    InstructorId = instructorId
                };

                db.Courses.Add(course);
                await db.SaveChangesAsync();

                // Act: Retrieve the course details
                var retrievedCourse = await db.Courses.FirstOrDefaultAsync(c => c.ClassNumber == "3750");

                // Assert: Verify the course details
                Assert.IsNotNull(retrievedCourse);
                Assert.AreEqual("Software Engineering II", retrievedCourse.Name);
                Assert.AreEqual("CS", retrievedCourse.Department);
                Assert.AreEqual(4, retrievedCourse.CreditHours);
                Assert.AreEqual(35, retrievedCourse.Capacity);
                Assert.AreEqual("Noorda 318", retrievedCourse.Location);
                Assert.AreEqual(new TimeSpan(8, 30, 0), retrievedCourse.StartTime);
                Assert.AreEqual(new TimeSpan(10, 30, 0), retrievedCourse.EndTime);
                Assert.AreEqual(instructorId, retrievedCourse.InstructorId);

                // Clean up: Remove the test data
                db.Courses.Remove(course);
                db.Users.Remove(instructor);
                await db.SaveChangesAsync();
            }
        }




        [TestMethod]
        public void UserRegistrationTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");
            // This will pull the variables we have in the ApplicationDbContext like Courses, Users, Registrations, etc. But these variables are empty because this doesn't pull from the database just the context
            //using (var db = new ApplicationDbContext(options))
            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                int N = db.Users.Count();
                User user = new User
                {
                    firstName = "User",
                    lastName = "Test",
                    email = "TestUser@weber.edu",
                    password = "12345678",
                    confirmPassword = "12345678",
                    BirthDate = DateTime.Parse("01-01-1999"),
                    userType = 'S'

                };
                db.Users.Add(user);

                db.SaveChanges();

                int M = db.Users.Count();

                Assert.IsTrue(M == N + 1);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }


        [TestMethod]
        public void AddCourseTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");


            // This will pull the variables we have in the ApplicationDbContext like Courses, Users, Registrations, etc. But these variables are empty because this doesn't pull from the database just the context
            //using (var db = new ApplicationDbContext(options))
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
                    InstructorId = 17, // Random Number, Null variable
                    MeetingDays = new List<string> { "Monday", "Wednesday", "Friday" }
                };

                db.Courses.Add(course);
                // Saves it to the database
                db.SaveChanges();

                int M = db.Courses.Count();

                // Assert: Verify expected results
                Assert.IsTrue(M == N + 1);

                db.Courses.Remove(course);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public async Task StudentRegisterCourse()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            // This will pull the datatypes we have in the ApplicationDbContext like Courses, Users, Registrations, etc. But the data is empty
            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                int N = db.Registrations.Count();

                //Recogniz
                var userId = 19; // Random Number
                var courseId = 11; // Random Number

                // This variables is null becasue the database is empty
                var course = await db.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

                // This variables is null becasue the database is empty
                var student = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

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
                    //Saves data to the database
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Error saving registration: {ex.Message}");
                }


                int M = db.Registrations.Count();

                // Assert: Verify expected results
                Assert.IsTrue(M == N + 1);

                db.Registrations.Remove(registration);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public async Task CalculateCredits_IsZero_IfNoClassesAreRegistered()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                AccountModel accountModel = new AccountModel(db); //Instantiate the class in order to get the method
                var userId = 106; // A random a@a.com user that shouldn't be registered for any classes

                User student;
                student = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                bool hadToUseADummyUser = false;
                if (student == null)
                {
                    hadToUseADummyUser = true;

                    student = new User()
                    {
                        firstName = "Dummy",
                        lastName = "User",
                        email = "a@a.a",
                        password = "password",
                        confirmPassword = "password",
                        BirthDate = DateTime.Parse("01-01-1999"),
                        userType = 'S'
                    };

                    db.Users.Add(student);
                    await db.SaveChangesAsync();
                }

                // Act: Calculate the credits
                var credits = accountModel.CalculateCredits(student.Id, "S");

                // Assert: Verify the credits
                Assert.AreEqual(0, credits);

                // Clean up: Remove the test data
                if (hadToUseADummyUser)
                {
                    db.Users.Remove(student);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
