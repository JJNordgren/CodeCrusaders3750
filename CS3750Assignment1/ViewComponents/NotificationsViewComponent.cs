using Microsoft.AspNetCore.Mvc;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CS3750Assignment1.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NotificationsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieve the current user's ID from the session.
            var userId = HttpContext.Session.GetInt32("UserId");

            // If no user is logged in, return an empty list.
            if (userId == null)
            {
                return View(Enumerable.Empty<Notification>());
            }

            // Query the Notifications table for notifications that belong to the current user and haven't been marked as deleted.
            List<Notification> notifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.Deleted)
                .ToListAsync();

            return View(notifications);
        }
    }
}
