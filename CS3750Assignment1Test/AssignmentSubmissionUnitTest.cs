using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using CS3750Assignment1.Pages;
using Microsoft.AspNetCore.Session;

namespace CS3750Assignment1Test
{
    [TestClass]
    public class AssignmentSubmissionUnitTest
    {
        // Helper: Fake IWebHostEnvironment Implementation
        // This should be the same for all tests
        public class FakeWebHostEnvironment : Microsoft.AspNetCore.Hosting.IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = "Development";
            public string ApplicationName { get; set; } = "TestApp";
            public string WebRootPath { get; set; }
            public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; }
            public string ContentRootPath { get; set; }
            public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
        }

        [TestMethod]
        public async Task OnPostAsync_TextAssignmentSubmission_ValidSubmission_RedirectsToPage()
        {
            // Include this in all tests to connect to the database (next 7 lines)
            DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(
                builder,
                "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;",
                null);
            var context = new ApplicationDbContext((DbContextOptions<ApplicationDbContext>)builder.Options);

            // Begin a transaction so changes are not permanently saved.
            // This makes temporary changes that we can then go and roll back after running the tests
            // I originally had it as a BeginPostAsync and manually deleted what I pushed to the database after each entry
            // but even though it worked, the Id counter was still being incremented which I did not want, so I found the transaction method.
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create and save instructor, course, assignment, student
                    // these are dummy entries which will be dropped after the test is run so no permanent changes are made to the database.
                    var instructor = new User
                    {
                        email = "dummy@dummy.com",
                        firstName = "Dummy",
                        lastName = "Instructor",
                        password = "password123",
                        BirthDate = DateTime.Now.AddYears(-30)
                    };
                    // I don't know why but my code hates me if I don't have these two lines after each one of these variable creations
                    context.Users.Add(instructor);
                    await context.SaveChangesAsync();

                    // Create a dummy course
                    var course = new Course
                    {
                        Department = "CS",
                        Name = "Introduction to Programming",
                        ClassNumber = "1001",
                        CreditHours = 3,
                        Capacity = 30,
                        Location = "Room 101",
                        MeetingDays = new List<string> { "Mon", "Wed", "Fri" },
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(10, 0, 0),
                        InstructorId = instructor.Id,
                        Instructor = instructor
                    };
                    context.Courses.Add(course);
                    await context.SaveChangesAsync();

                    // Create a dummy assignment
                    var assignment = new Assignment
                    {
                        Title = "Text Assignment",
                        Description = "Submit your text assignment.",
                        DueDate = DateTime.Now.AddDays(7),
                        MaxPoints = 100,
                        AssignmentType = 'T',
                        CourseId = course.Id,
                        InstructorId = instructor.Id,
                        Course = course,
                        Instructor = instructor
                    };
                    context.Assignment.Add(assignment);
                    await context.SaveChangesAsync();

                    // create a dummy student user
                    var student = new User
                    {
                        email = "student@dummy.com",
                        firstName = "Stu",
                        lastName = "Dent",
                        password = "password123",
                        BirthDate = DateTime.Now.AddYears(-20)
                    };
                    context.Users.Add(student);
                    await context.SaveChangesAsync();

                    // Setup Fake Web Host Environment and PageModel
                    var fakeEnvironment = new FakeWebHostEnvironment { WebRootPath = Path.GetTempPath() };
                    var pageModel = new AssignmentSubmissionModel(context, fakeEnvironment);

                    // Setup HttpContext and mock ISession using TryGetValue
                    var httpContext = new DefaultHttpContext();
                    var mockSession = new Mock<ISession>();
                    mockSession.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny))
                        .Returns((string key, out byte[] value) => { value = BitConverter.GetBytes(student.Id); return true; });
                    mockSession.Setup(s => s.TryGetValue("CourseId", out It.Ref<byte[]>.IsAny))
                        .Returns((string key, out byte[] value) => { value = BitConverter.GetBytes(course.Id); return true; });
                    httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = mockSession.Object });
                    pageModel.PageContext = new PageContext { HttpContext = httpContext };

                    // Prepare Submission (Text-based)
                    pageModel.Submission = new Submission
                    {
                        AssignmentId = assignment.Id,
                        TextSubmission = "This is my text submission."
                    };
                    // Set the navigation property so that EF links the submission with the student.
                    pageModel.Submission.Student = student;

                    // Call OnPostAsync from the page
                    var result = await pageModel.OnPostAsync();

                    // Validate Redirect and DB Persistence
                    Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
                    var submissionInDb = await context.Submissions
                        .FirstOrDefaultAsync(s => s.AssignmentId == assignment.Id && s.UserId == student.Id);
                    Assert.IsNotNull(submissionInDb);
                    Assert.AreEqual("This is my text submission.", submissionInDb.TextSubmission);
                    Assert.AreEqual(assignment.MaxPoints, submissionInDb.TotalPoints);
                    Assert.AreNotEqual(default(DateTime), submissionInDb.SubmissionTime);
                }
                finally
                {
                    // Roll back the transaction so no changes are kept.
                    await transaction.RollbackAsync();
                }
            }
        }

        [TestMethod]
        public async Task OnPostAsync_FileAssignmentSubmission_ValidSubmission_RedirectsToPage()
        {
            // Include this in all tests to connect to the database (next 7 lines)
            DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(
                builder,
                "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;",
                null);
            var context = new ApplicationDbContext((DbContextOptions<ApplicationDbContext>)builder.Options);

            // Begin transaction so changes are not permanently saved
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create an instructor user type
                    var instructor2 = new User
                    {
                        email = "dummy2@dummy.com",
                        firstName = "Dummy2",
                        lastName = "Instructor",
                        password = "password123",
                        BirthDate = DateTime.Now.AddYears(-30)
                    };
                    context.Users.Add(instructor2);
                    await context.SaveChangesAsync();

                    // create a dummy course
                    var course2 = new Course
                    {
                        Department = "CS",
                        Name = "Data Structures",
                        ClassNumber = "2002",
                        CreditHours = 3,
                        Capacity = 30,
                        Location = "Room 202",
                        MeetingDays = new List<string> { "Tue", "Thu" },
                        StartTime = new TimeSpan(11, 0, 0),
                        EndTime = new TimeSpan(12, 0, 0),
                        InstructorId = instructor2.Id,
                        Instructor = instructor2
                    };
                    context.Courses.Add(course2);
                    await context.SaveChangesAsync();

                    // Create another dummy assignment of fily type upload
                    var assignment = new Assignment
                    {
                        Title = "File Assignment",
                        Description = "Submit your file assignment.",
                        DueDate = DateTime.Now.AddDays(7),
                        MaxPoints = 50,
                        AssignmentType = 'F',
                        CourseId = course2.Id,
                        InstructorId = instructor2.Id,
                        Course = course2,
                        Instructor = instructor2
                    };
                    context.Assignment.Add(assignment);
                    await context.SaveChangesAsync();

                    // Create another dummy student user
                    var student = new User
                    {
                        email = "student2@dummy.com",
                        firstName = "Stu2",
                        lastName = "Dent",
                        password = "password123",
                        BirthDate = DateTime.Now.AddYears(-20)
                    };
                    context.Users.Add(student);
                    await context.SaveChangesAsync();

                    // Setup Fake Web Host Environment with a temporary folder
                    string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    // creates a temporary folder at runtime to store a file upload, will be deleted after the test runs
                    Directory.CreateDirectory(tempFolder);
                    var fakeEnvironment = new FakeWebHostEnvironment { WebRootPath = tempFolder };

                    var pageModel = new AssignmentSubmissionModel(context, fakeEnvironment);

                    // Setting up the moq session
                    var httpContext = new DefaultHttpContext();
                    var mockSession = new Mock<ISession>();
                    mockSession.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny))
                        .Returns((string key, out byte[] value) => { value = BitConverter.GetBytes(student.Id); return true; });
                    mockSession.Setup(s => s.TryGetValue("CourseId", out It.Ref<byte[]>.IsAny))
                        .Returns((string key, out byte[] value) => { value = BitConverter.GetBytes(course2.Id); return true; });
                    httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = mockSession.Object });
                    pageModel.PageContext = new PageContext { HttpContext = httpContext };

                    // Create a Fake File for File Upload, had to look this up
                    // Kind of turned into one of those "Okay, it works, now don't touch it!" moments
                    var fileContent = "Dummy file content";
                    var stream = new MemoryStream();
                    var writer = new StreamWriter(stream);
                    writer.Write(fileContent);
                    writer.Flush();
                    stream.Position = 0;
                    var formFile = new FormFile(stream, 0, stream.Length, "UploadedFiles", "testfile.txt")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "text/plain"
                    };

                    pageModel.UploadedFiles = new List<IFormFile> { formFile };
                    pageModel.Submission = new Submission
                    {
                        AssignmentId = assignment.Id
                    };
                    // Set the navigation property for the student.
                    pageModel.Submission.Student = student;

                    // Call OnPostAsync from the assignment submission file
                    var result = await pageModel.OnPostAsync();

                    // Validate Redirect and DB Persistence
                    Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
                    var submissionInDb = await context.Submissions
                        .FirstOrDefaultAsync(s => s.AssignmentId == assignment.Id && s.UserId == student.Id);
                    Assert.IsNotNull(submissionInDb);
                    Assert.IsNotNull(submissionInDb.FilePath);
                    Assert.IsTrue(submissionInDb.FilePath.Any());

                    // Cleanup the temporary folder.
                    Directory.Delete(tempFolder, true);
                }
                finally
                {
                    // Roll back the transaction so that nothing is permanently saved.
                    await transaction.RollbackAsync();
                }
            }
        }
    }
}
