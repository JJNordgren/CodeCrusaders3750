using CS3750Assignment1.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe.V2;
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
        public string ReceiptPath { get; set; } = default!;


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
            ReceiptPath = GenerateReceipt(user, amount);

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

        // Add the GenerateReceipt method to create and display the receipt
        private string GenerateReceipt(User user, long amount) { 
            var uniqueFileName = Guid.NewGuid().ToString() + "_receipt.pdf";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Receipts");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
                Font boldLargeFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);

                // Create a chunk with the bold and large font
                Chunk boldLargeChunk = new Chunk("PAYMENT RECEIPT", boldLargeFont);

                // Add the chunk to the document
                document.Add(boldLargeChunk);
                document.Add(new Paragraph(""));
                document.Add(new Paragraph("Tuition Payment"));
                document.Add(new Paragraph("Customer Name: " + user.firstName + " " + user.lastName));
                document.Add(new Paragraph("Customer Phone Number: " + user.phone));
                document.Add(new Paragraph("Customer Email: " + user.email));
                document.Add(new Paragraph("Amount Paid: $" + amount));
                document.Add(new Paragraph("Remaining Balance: $" + (Tuition - totalAmountPaid)));

                document.Close();
                writer.Close();
            }

            return uniqueFileName;
        }

    }
}
