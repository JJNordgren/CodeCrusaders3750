using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

// Add necessary using directive for ReceiptDetails
//using StripePaymentProcessing;

namespace CS3750Assignment1.Pages
{
    public class SuccessModel : PageModel
    {
       private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public IList<Course> Course { get; set; } = default!;
        public IList<CS3750Assignment1.Models.Registration> Registrations { get; set; } = default!;
        public int Credits { get; set; }
        public double Tuition { get; set; }

        public long amount { get; set; } = default!;

        public double totalAmountPaid { get; set; }


        public SuccessModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(long Amount)
        {
            // Store local amount variable equal to the amount passed in
            amount = Amount;

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user == null)
            {
                return RedirectToPage("/LoginPage");
            }
           
            user.amountPaid += amount;
            await _context.SaveChangesAsync();
            totalAmountPaid = user.amountPaid ?? 0.0;

            // Retrieves courses
            Course = await _context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();

            // Retrieves the registrations table
            Registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .ToListAsync();

            Credits = CalculateCredits(userId.Value);
            Tuition = Credits * 100;
            Tuition = Tuition - totalAmountPaid;

            // Generate and display the receipt
            //GenerateReceipt(user, amount);

            return Page();
        }

        public int CalculateCredits(int userId)
        {
            var userType = HttpContext.Session.GetString("UserType");

            int credits = 0;

            if (userType == "S")
            {
                var studentRegistrations = Registrations.Where(r => r.UserId == userId);

                foreach (var registration in studentRegistrations)
                {
                    credits += registration.Course.CreditHours;
                }
            }

            return credits;
        }

        //// Add the GenerateReceipt method to create and display the receipt
        //private void GenerateReceipt(User user, long amount)
        //{
        //    // Create an instance of ReceiptDetails and populate it with necessary information
        //    var receiptDetails = new ReceiptDetails
        //    {
        //        AccountType = "Credit",
        //        ApplicationCryptogram = "9F26",
        //        ApplicationPreferredName = "Visa",
        //        AuthorizationCode = "123456",
        //        AuthorizationResponseCode = "00",
        //        CVM = "PIN",
        //        DedicatedFileName = "A0000000031010",
        //        TSI = "F800",
        //        TVR = "0000008000"
        //    };

        //    // Display receipt details (this is a simple example, you can expand it as needed)
        //    Console.WriteLine("Receipt Details:");
        //    Console.WriteLine($"Account Type: {receiptDetails.AccountType}");
        //    Console.WriteLine($"Application Cryptogram: {receiptDetails.ApplicationCryptogram}");
        //    Console.WriteLine($"Application Preferred Name: {receiptDetails.ApplicationPreferredName}");
        //    Console.WriteLine($"Authorization Code: {receiptDetails.AuthorizationCode}");
        //    Console.WriteLine($"Authorization Response Code: {receiptDetails.AuthorizationResponseCode}");
        //    Console.WriteLine($"CVM: {receiptDetails.CVM}");
        //    Console.WriteLine($"Dedicated File Name: {receiptDetails.DedicatedFileName}");
        //    Console.WriteLine($"TSI: {receiptDetails.TSI}");
        //    Console.WriteLine($"TVR: {receiptDetails.TVR}");

        //    // You can also add code here to render the receipt details in the view if needed
        //}

    }
}
