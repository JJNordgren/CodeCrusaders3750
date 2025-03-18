using Microsoft.AspNetCore.Mvc;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.Extensions.Logging;

namespace CS3750Assignment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(ApplicationDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNotification(int id)
        {
            _logger.LogInformation($"Received request to delete notification with ID: {id}");

            var notification = _context.Notification.Find(id);
            if (notification == null)
            {
                _logger.LogWarning($"Notification with ID: {id} not found");
                return NotFound();
            }

            _context.Notification.Remove(notification);
            _context.SaveChanges();

            _logger.LogInformation($"Notification with ID: {id} deleted successfully");
            return NoContent();
        }
    }
}

