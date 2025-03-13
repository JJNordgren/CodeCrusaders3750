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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;


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


        [TestMethod] //Kaylee Unit Test 1
        public void SuccessfulUserRegistrationTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                User NewUser = new User
                {
                    firstName = "User",
                    lastName = "Test",
                    email = "TestUser@weber.edu",
                    password = "12345678",
                    confirmPassword = "12345678",
                    BirthDate = DateTime.Parse("01-01-1999"),
                    userType = 'S'
                };

                // Create an instance of RegisterModel
                var registerModel = new RegisterModel(db);

                // ACT
                registerModel.UnitTestSuccessfulRegistration(NewUser);

                // VERIFY
                var registeredUser = db.Users.FirstOrDefault(u => u.email == NewUser.email);
                Assert.IsNotNull(registeredUser);

                db.Users.Remove(registeredUser);
                db.SaveChanges();
            }
        }

        [TestMethod] //Kaylee Unit Test 2
        public void UnsuccessfulUserRegistrationTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {

                // Arrange: Set up test data with mismatched passwords
                User NewUser = new User
                {
                    firstName = "User",
                    lastName = "Test",
                    email = "usertest@gmail.com",
                    password = "12345678",
                    confirmPassword = "notMatchingPass", //passwords does not match, invalid
                    BirthDate = DateTime.Parse("01-01-1999"),
                    userType = 'S'
                };

                // Manually validate the User model
                var validationContext = new ValidationContext(NewUser);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(NewUser, validationContext, validationResults, true);
               
                // Assert that the validation fails
                Assert.IsFalse(isValid, "User validation should fail but passed.");

                // Make sure the password mismatch is caught
                Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage == "The password and confirmation password do not match."),
                    "Expected password mismatch validation error not found.");

                // Create an instance of RegisterModel
                var registerModel = new RegisterModel(db);

                // ACT: Expect an exception
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    registerModel.UnitTestSuccessfulRegistration(NewUser);
                }, "Expected InvalidOperationException due to validation errors, but no exception was thrown.");

                // VERIFY: Ensure user was NOT added to the database
                var registeredUser = db.Users.FirstOrDefault(u => u.email == NewUser.email);
                Assert.IsNull(registeredUser, "User should not be in the database after failed validation.");
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
