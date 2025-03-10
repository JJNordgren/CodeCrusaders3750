using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1Test
{
    [TestClass]
    public class UnitTests
    {

        /*
        
        PUT THIS IN EACH [TestMethod] you create, should work as is

        DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
        DbContextOptionsBuilder builder = new DbContextOptionsBuilder(options);
        SqlServerDbContextOptionsExtensions.UseSqlServer(builder, "Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;", null);
        var _context = new ApplicationDbContext((DbContextOptions<ApplicationDbContext>)builder.Options);
        
         */




        [TestMethod]
        public async Task EditUserProfileTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=3750_Spr25_CodeCrusaders;User ID=3750_Spr25_CodeCrusaders;Password=Burrito9*;TrustServerCertificate=True;");

            using (var db = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Arrange: Set up test data
                var user = new User
                {
                    firstName = "Jane",
                    lastName = "Smith",
                    email = "janesmith@weber.edu",
                    password = "securepassword",
                    confirmPassword = "securepassword",
                    BirthDate = DateTime.Parse("1990-05-15"),
                    userType = 'S'
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();

                // Act: Edit the user's profile information
                var retrievedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                Assert.IsNotNull(retrievedUser);

                retrievedUser.firstName = "Janet";
                retrievedUser.lastName = "Doe";
                retrievedUser.email = "janetdoe@weber.edu";

                db.Users.Update(retrievedUser);
                await db.SaveChangesAsync();

                // Retrieve the updated user
                var updatedUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

                // Assert: Verify the updated details
                Assert.IsNotNull(updatedUser);
                Assert.AreEqual("Janet", updatedUser.firstName);
                Assert.AreEqual("Doe", updatedUser.lastName);
                Assert.AreEqual("janetdoe@weber.edu", updatedUser.email);

                // Clean up: Remove the test data
                db.Users.Remove(updatedUser);




















            }
}
