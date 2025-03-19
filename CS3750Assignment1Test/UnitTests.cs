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
using Microsoft.Extensions.Logging;


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
        //JaKota's Test 1
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


        //JaKota's Test 2
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
        //[TestMethod]
        //public async Task ViewClassDetailsTest()

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



        [TestMethod] // Will's Unit Test 1
        public async Task AddCourseTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");
            // Create a logger for the correct Model
            ILogger<CreateAClassModel> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CreateAClassModel>();

            // This will pull the variables we have in the ApplicationDbContext like Courses, Users, Registrations,etc.
            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                CreateAClassModel createaclassmodel = new CreateAClassModel(db, logger);
                int N = await db.Courses.CountAsync();

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

                bool result = await createaclassmodel.CreateAClass(course);
                await db.SaveChangesAsync();

                int M = await db.Courses.CountAsync();

                // Assert: Verify expected results
                Assert.IsTrue(M == N + 1);

                db.Courses.Remove(course);
                await db.SaveChangesAsync();
            }
        }

        [TestMethod] // Will's Unit Test 2
        public async Task StudentRegisterCourse()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            // Create a logger for the correct Model
            ILogger<StudentClassesModel> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<StudentClassesModel>();


            // This will pull the datatypes we have in the ApplicationDbContext like Courses, Users, Registrations, etc.
            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                StudentClassesModel studentclassesmodel = new StudentClassesModel(db, logger);
                int N = await db.Registrations.CountAsync();

                var userId = 39; // Random Number
                var courseId = 10; // Random Number

                var course = await db.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

                var student = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var registration = new Registration
                {
                    UserId = userId,
                    Student = student,
                    CourseId = courseId,
                    Course = course
                };

                bool result = await studentclassesmodel.AddARegistration(registration);
                await db.SaveChangesAsync();

                int M = await db.Registrations.CountAsync();

                // Assert: Verify expected results
                Assert.IsTrue(M == N + 1);

                db.Registrations.Remove(registration);
                await db.SaveChangesAsync();
            }
        }

        // Matt's unit test 1
        [TestMethod]
        public async Task CalculateCredits_IsZero_IfNoClassesAreRegistered()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                AccountModel accountModel = new AccountModel(db); //Instantiate the class in order to get the method
                var userId = 42; // A random a@a.com user that shouldn't be registered for any classes

                User student;
                student = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                bool hadToUseADummyUser = false;
                if (student == null)
                {
                    hadToUseADummyUser = true;

                    student = new User()
                    {
                        firstName = "Nicholas",
                        lastName = "Deer",
                        email = "a@a.a",
                        password = "deleteMe",
                        confirmPassword = "deleteMe",
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

                //Clean up: Remove the test data
                if (hadToUseADummyUser)
                {
                    db.Users.Remove(student);
                    await db.SaveChangesAsync();
                }
            }
        }

        // Matt's second unit test
        [TestMethod]
        public async Task AddAsignmentAsync_CreatesNewAssignmentSuccessfully()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                AddAssignmentModel addAssignment = new AddAssignmentModel(db); //Instantiate the class in order to get the method

                // Arrange: Set up test data
                int instructorId = 5;
                int courseId = 10;

                User instructor = await db.Users.FirstOrDefaultAsync(u => u.Id == instructorId);
                Course course = await db.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

                bool hadToUseDummyInstructor = false;
                if (instructor == null)
                {
                    hadToUseDummyInstructor = true;
                    instructor = new User()
                    {
                        firstName = "Sam",
                        lastName = "Gamgee",
                        email = "b@b.b",
                        password = "deleteMe",
                        confirmPassword = "deleteMe",
                        BirthDate = DateTime.Parse("02-02-2002"),
                        userType = 'T'
                    };

                    db.Users.Add(instructor);
                    await db.SaveChangesAsync();

                    instructorId = instructor.Id;
                }

                bool hadToUseDummyCourse = false;
                if (course == null)
                {
                    hadToUseDummyCourse = true;
                    course = new Course()
                    {
                        Department = "CS",
                        Name = "Scripting Languages",
                        ClassNumber = "3030",
                        CreditHours = 3,
                        Capacity = 30,
                        Location = "NB 202",
                        StartTime = DateTime.Parse("08:00 AM").TimeOfDay,
                        EndTime = DateTime.Parse("09:00 PM").TimeOfDay,
                        InstructorId = instructorId,
                        MeetingDays = new List<string> { "Monday", "Wednesday", "Friday" }
                    };

                    db.Courses.Add(course);
                    await db.SaveChangesAsync();

                    courseId = course.Id;
                }

                int oldAssignmentCount = await db.Assignment.Where(Assignment => Assignment.CourseId == courseId && Assignment.InstructorId == instructorId).CountAsync();

                // Act: Add a new assignment
                Assignment assignment = new Assignment()
                {
                    Title = "Really boring script",
                    Description = "Write a script that does nothing",
                    DueDate = DateTime.Parse("05-05-2025"),
                    MaxPoints = 1000,
                    AssignmentType = 'T'
                };

                await addAssignment.AddAsignmentAsync(assignment, courseId, instructorId);

                int newAssignmentCount = await db.Assignment.Where(Assignment => Assignment.CourseId == courseId && Assignment.InstructorId == instructorId).CountAsync();

                // Assert: Verify the assignment was added
                Assert.AreEqual(oldAssignmentCount + 1, newAssignmentCount);

                // Clean up: Remove the test data, starting with the most dependant to least
                //Assignment a = await db.Assignment.Where(assignment => assignment.CourseId == courseId && assignment.InstructorId == instructorId).OrderByDescending(a => a.Id).FirstAsync();
                db.Assignment.Remove(assignment);
                if (hadToUseDummyCourse)
                {
                    db.Courses.Remove(course);
                }
                if (hadToUseDummyInstructor)
                {
                    db.Users.Remove(instructor);
                }

                await db.SaveChangesAsync();
            }
        }


    }
}
