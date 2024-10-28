using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Tickets.Models;
using YourNamespace.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Dashboard(DateTime? startDate, DateTime? endDate, int? assignedUserId, string assignedByEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    return RedirectToAction("Login", "Account");
                }

                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                var ticketsQuery = _context.Tickets
                    .Where(t => t.CreatedAt >= start && t.CreatedAt <= end);

                if (assignedUserId.HasValue)
                {
                    ticketsQuery = ticketsQuery.Where(t => t.AssignedUserId == assignedUserId);
                }

                if (!string.IsNullOrEmpty(assignedByEmail))
                {
                    ticketsQuery = ticketsQuery.Where(t => t.AssignedByEmail == assignedByEmail);
                }

                var tickets = ticketsQuery.ToList();
                var highPriorityCount = tickets.Count(t => t.Priority == "High");
                var mediumPriorityCount = tickets.Count(t => t.Priority == "Medium");
                var lowPriorityCount = tickets.Count(t => t.Priority == "Low");

                var statusChartData = tickets
                    .GroupBy(t => t.Status)
                    .Select(g => new StatusChartData
                    {
                        Status = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .ToList();

                var ticketsCreatedByDate = tickets
                    .GroupBy(t => t.CreatedAt.Date)
                    .Select(g => new TicketsCreatedByDate
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Date)
                    .ToList();

                var assignedUsers = _context.Users.ToList();
                var assignedByUsers = _context.Users.ToList();

                ViewBag.UserOptions = new SelectList(assignedUsers, "Id", "FullName");
                ViewBag.LeadOptions = new SelectList(assignedByUsers, "Email", "FullName");

                var dashboardViewModel = new DashboardViewModel
                {
                    HighPriorityTickets = highPriorityCount,
                    MediumPriorityTickets = mediumPriorityCount,
                    LowPriorityTickets = lowPriorityCount,
                    StatusChartData = statusChartData,
                    TicketsCreatedByDate = ticketsCreatedByDate,
                    StartDate = start,
                    EndDate = end,
                    AssignedUserId = assignedUserId,
                    AssignedByEmail = assignedByEmail,
                    AssignedUsers = assignedUsers,
                    AssignedByUsers = assignedByUsers
                };

                ViewBag.LeadOptions = _context.Users
                    .Where(u => u.Role != "Admin" && u.Role != "User")
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Email
                    })
                    .ToList();

                ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                    .Cast<StatusEnum>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    });

                ViewBag.UserOptions = _context.Users
                    .Where(u => u.Role != "Admin" && u.Role != "TeamLead")
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Email
                    })
                    .ToList();

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the dashboard");
                return RedirectToAction("Error");
            }
        }

        public IActionResult TeamLeadDashboard(DateTime? startDate, DateTime? endDate, int? assignedUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    return RedirectToAction("Login", "Account");
                }

                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                var ticketsQuery = _context.Tickets
                    .Where(t => t.CreatedAt >= start && t.CreatedAt <= end);

                if (assignedUserId.HasValue)
                {
                    ticketsQuery = ticketsQuery.Where(t => t.AssignedUserId == assignedUserId);
                }

                var tickets = ticketsQuery.ToList();
                var ticketsCreatedByDate = tickets
                    .GroupBy(t => t.CreatedAt.Date)
                    .Select(g => new TicketsCreatedByDate
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Date)
                    .ToList();

                var model = new DashboardViewModel
                {
                    TicketsCreatedByDate = ticketsCreatedByDate,
                    StartDate = start,
                    EndDate = end,
                };

                ViewBag.UserOptions = _context.Users
                    .Where(u => u.Role != "Admin" && u.Role != "TeamLead")
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Email
                    })
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the TeamLead dashboard");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> UserDashboard(int Id)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                {
                    return RedirectToAction("Login", "Account");
                }

                var tickets = await _context.Tickets
                          .Where(t => t.AssignedUserId == Id)
                          .Select(t => new Ticket
                          {
                              Id = t.Id,
                              Subject = t.Subject,
                              TicketBody = t.TicketBody,
                              CreatedAt = t.CreatedAt,
                              Priority = t.Priority,
                              Status = t.Status,
                              AssignedByEmail = _context.Users
.Where(u => u.Id.ToString() == t.AssignedByEmail)  // Convert int 'Id' to string for comparison
.Select(u => u.Email)
.FirstOrDefault()
                          })
                          .ToListAsync();

                return View(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the User dashboard");
                return RedirectToAction("Error");
            }
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
