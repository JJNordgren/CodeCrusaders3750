using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Models;
using Stripe;
using Stripe.Checkout;

namespace CS3750Assignment1.Pages
{
    public class AccountModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public IList<Course> Course { get; set; } = default!;
        public IList<CS3750Assignment1.Models.Registration> Registrations { get; set; } = default!;
        public int Credits { get; set; }
        public double Tuition { get; set; }

        [BindProperty]
        public long amount { get; set; } = default!;

        public double totalAmountPaid { get; set; }

        public AccountModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            return Page();
        }

        // This method is used to load the data for the page and check if the user is logged in to prevent unauthorized access
        private async Task LoadDataAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                RedirectToPage("/LoginPage");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                RedirectToPage("/LoginPage");
            }

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

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDataAsync();

            // Checks for decimals, decimals result in an error because they are set equal to a long datatype which doesn't support decimals. 
            // Stripe accepts the data type long for the amount
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "The amount must be a positive whole number.");
                return Page();
            }

            // Checks if the amount is less than 1
            if (amount < 1)
            {
                ModelState.AddModelError(string.Empty, "The amount must be a positive whole number.");
                return Page();
            }

            // In the OnGetAsync we set Tuition = Tuition - totalAmountPaid; So below we check if the remaining balance is less than the amount the user is trying to pay
            if (amount > (decimal)Tuition)
            {
                ModelState.AddModelError(string.Empty, "The amount exceeds the remaining tuition balance.");
                return Page();
            }

            StripeConfiguration.ApiKey = "sk_test_51QpBLxB7E0YPWfnAnQvYRQbYhYRtwZ1CT8oPSWEq5LNNtsM7gBwUvMjeqgMJ49CiyUXPDxYBENe0t0d5btpNnKdL00xEEQZpYK";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = amount * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Tuition",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"http://localhost:5097/Success?amount={amount}",
                CancelUrl = "http://localhost:5097/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            Response.Redirect(session.Url);


            return Page();
        }
    }
}
