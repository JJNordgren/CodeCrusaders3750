using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using CS3750Assignment1.Pages;

namespace CS3750Assignment1Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public async Task EditUserProfileTest()
        {
            
            
        }
        [TestMethod]
        public async Task CheckAmountPaid()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                SuccessModel accountModel = new SuccessModel(db); //Instantiate the class in order to get the method
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

                // Act: Calculate the amount paid
                var paid = accountModel.UpdateAmountPaidAsync(student, 900);

                // Assert: Verify the amount paid
                Assert.AreEqual(900, student.amountPaid);

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
