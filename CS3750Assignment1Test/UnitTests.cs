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
        public async Task UpdateUserAddress()
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
