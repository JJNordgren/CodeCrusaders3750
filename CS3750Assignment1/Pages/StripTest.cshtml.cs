using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;
using Stripe;

namespace CS3750Assignment1.Pages
{
    public class StripTestModel : PageModel
    {
        public void OnGet()
        {
            StripeConfiguration.ApiKey = "sk_test_51QpBLxB7E0YPWfnAnQvYRQbYhYRtwZ1CT8oPSWEq5LNNtsM7gBwUvMjeqgMJ49CiyUXPDxYBENe0t0d5btpNnKdL00xEEQZpYK";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
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
                SuccessUrl = "http://localhost:4242/success",
                CancelUrl = "http://localhost:4242/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return;
        }

        public void OnPost() {
            StripeConfiguration.ApiKey = "sk_test_51QpBLxB7E0YPWfnAnQvYRQbYhYRtwZ1CT8oPSWEq5LNNtsM7gBwUvMjeqgMJ49CiyUXPDxYBENe0t0d5btpNnKdL00xEEQZpYK";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
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
                SuccessUrl = "http://localhost:4242/success",
                CancelUrl = "http://localhost:4242/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            Response.Redirect(session.Url);
            return;
        }
    }
}
