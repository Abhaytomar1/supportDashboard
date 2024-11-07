using Microsoft.AspNetCore.Mvc;     //Provides classes for building controllers and handling HTTP requests in an ASP.NET Core MVC application.
using Tickets.Models;                //This is a custom namespace where your ticket-related models, such as Ticket, are likely defined.
using System.Threading.Tasks;
using YourNamespace.Data;                 //A placeholder namespace where your ApplicationDbContext class is located, which handles the database interactions.
using Microsoft.EntityFrameworkCore;   // This is the Entity Framework Core library, which allows interacting with the database using object-relational mapping (ORM).
using Microsoft.AspNetCore.Mvc.Rendering;  
using System.Linq;
using YourNamespace.Models;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace Tickets.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        //// GET: Ticket/Create
        //public IActionResult Create()
        //{
        //    // Populate Status options for dropdown
        //    ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
        //                                .Cast<StatusEnum>()
        //                                .Select(e => new SelectListItem
        //                                {
        //                                    Value = e.ToString(),
        //                                    Text = e.ToString()
        //                                });

        //    return View();
        //}
        // GET: Ticket/Create
        public IActionResult Create()
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString(),
                                            Selected = e == StatusEnum.Open,
                                            Disabled = e == StatusEnum.Pending ||
                                           e == StatusEnum.Resolved ||
                                           e == StatusEnum.Closed
                                        });

            //// Populate Users for dropdown
            //ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
            //{
            //    Value = u.Id.ToString(),
            //    Text = u.Email // or any other user property you want to display
            //}).ToList();
            // Populate Users for dropdown, excluding admins and team leads
            ViewBag.UserOptions = _context.Users
                .Where(u => u.Role != "Admin" && u.Role != "TeamLead") // Adjust based on your role management
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Email // or any other user property you want to display
                })
                .ToList();

            ViewBag.LeadOptions = _context.Users
               .Where(u => u.Role != "Admin" && u.Role != "User") // Adjust based on your role management
               .Select(u => new SelectListItem
               {
                   Value = u.Id.ToString(),
                   Text = u.Email // or any other user property you want to display
               })
               .ToList();


            return View();
        }


        //// POST: Ticket/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Ticket ticket)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(ticket);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Re-populate Status options if validation fails
        //    ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
        //                                .Cast<StatusEnum>()
        //                                .Select(e => new SelectListItem
        //                                {
        //                                    Value = e.ToString(),
        //                                    Text = e.ToString()
        //                                });

        //    return View(ticket);
        //}
        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create1(Ticket ticket)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("TeamLeadDashboard", "Home", new { Id = ticket.AssignedByEmail });
                }
                catch (DbUpdateException ex)
                {
                    // Check if the exception is related to a UNIQUE constraint violation on TicketNo
                    if (ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") == true)
                    {
                        ModelState.AddModelError("TicketNo", "A ticket with this number already exists. Please use a different ticket number.");
                    }
                    else
                    {
                        // Handle other DbUpdateExceptions
                        ModelState.AddModelError("", "An error occurred while saving the ticket. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other unexpected exceptions
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }
            ViewBag.LeadOptions = _context.Users
              .Where(u => u.Role != "Admin" && u.Role != "User") // Adjust based on your role management
              .Select(u => new SelectListItem
              {
                  Value = u.Id.ToString(),
                  Text = u.Email // or any other user property you want to display
              })
              .ToList();
            // Re-populate User options if validation fails or an exception occurs
            ViewBag.UserOptions = _context.Users
                                          .Select(u => new SelectListItem
                                          {
                                              Value = u.Id.ToString(),
                                              Text = u.Email // Or any other field for displaying users
                                          }).ToList();

            // Re-populate Status options if validation fails or an exception occurs
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            return RedirectToAction("TeamLeadDashboard", "Home", new { Id = ticket.AssignedByEmail });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("TeamLeadDashboard", "Home", new { Id = ticket.AssignedByEmail });
                }
                catch (DbUpdateException ex)
                {
                    // Check for UNIQUE constraint violations or other database errors
                    if (ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") == true)
                    {
                        ModelState.AddModelError("TicketNo", "A ticket with this number already exists. Please use a different ticket number.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the ticket. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Repopulate ViewBag data in case of validation failure
            ViewBag.LeadOptions = _context.Users
                .Where(u => u.Role != "Admin" && u.Role != "User")
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Email
                })
                .ToList();

            ViewBag.UserOptions = _context.Users
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

            // Return the view with the validation errors and data
            return View(ticket);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Ticket ticket)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(ticket);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("TeamLeadDashboard", "Home", new { Id = ticket.AssignedUserId });
        //    }

        //    // Re-populate Status options if validation fails
        //    ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
        //                                .Cast<StatusEnum>()
        //                                .Select(e => new SelectListItem
        //                                {
        //                                    Value = e.ToString(),
        //                                    Text = e.ToString()
        //                                });

        //    return View(ticket);
        //}


        // GET: Ticket/Index

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Create a list to hold the tickets
            var tickets = new List<TicketReportDto>();

            // Set the parameters for the stored procedure
            var startDateParam = new SqlParameter("@StartDate", startDate.HasValue ? (object)startDate.Value : DBNull.Value);
            var endDateParam = new SqlParameter("@EndDate", endDate.HasValue ? (object)endDate.Value : DBNull.Value);

            // Execute the stored procedure and retrieve the results
            var ticketResults = await _context.TicketReportDtos
                .FromSqlRaw("EXEC GetTicketsByDateRange @StartDate, @EndDate", startDateParam, endDateParam)
                .ToListAsync();

            // Map the results to the TicketReportDto
            tickets = ticketResults.Select(t => new TicketReportDto
            {
                Id = t.Id,
                Subject = t.Subject,
                TicketBody = t.TicketBody,
                CreatedAt = t.CreatedAt,
                Priority = t.Priority,
                Status = t.Status, // This will now be a string (Open, Pending, Resolved, or Closed)
                AssignedUserEmail = t.AssignedUserEmail,
                AssignedByEmail = t.AssignedByEmail,
                // Add other necessary properties if any
            }).ToList();

            // Pass the startDate and endDate to the view using ViewBag
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Return the filtered list to the view
            return View(tickets); // Ensure the view is updated to use IEnumerable<TicketReportDto>
        }



        // GET: Ticket/PriorityDistribution
        public async Task<IActionResult> PriorityDistribution()
        {

            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            // Fetch data from the database
            var highPriorityTickets = await _context.Tickets.Where(t => t.Priority == "High").ToListAsync();
            var mediumPriorityTickets = await _context.Tickets.Where(t => t.Priority == "Medium").ToListAsync();
            var lowPriorityTickets = await _context.Tickets.Where(t => t.Priority == "Low").ToListAsync();

            var model = new TicketPriorityViewModel
            {
                HighPriorityCount = highPriorityTickets.Count,
                MediumPriorityCount = mediumPriorityTickets.Count,
                LowPriorityCount = lowPriorityTickets.Count,
                HighPriorityTickets = highPriorityTickets,
                MediumPriorityTickets = mediumPriorityTickets,
                LowPriorityTickets = lowPriorityTickets
            };

            return View(model);
        }

        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.LeadOptions = _context.Users
             .Where(u => u.Role != "Admin" && u.Role != "User") // Adjust based on your role management
             .Select(u => new SelectListItem
             {
                 Value = u.Id.ToString(),
                 Text = u.Email // or any other user property you want to display
             })
             .ToList();

            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });
            // Populate Users for dropdown
            ViewBag.UserOptions = _context.Users
               .Where(u => u.Role != "Admin" && u.Role != "TeamLead") // Adjust based on your role management
               .Select(u => new SelectListItem
               {
                   Value = u.Id.ToString(),
                   Text = u.Email // or any other user property you want to display
               })
               .ToList();



            return View(ticket);
        }

        //// POST: Ticket/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,TicketNo,Subject,TicketBody,Priority,Status,Deadline,AssignedUserId")] Ticket ticket)
        //{
        //    if (id != ticket.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(ticket);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TicketExists(ticket.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Re-populate Status options if validation fails
        //    ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
        //                                .Cast<StatusEnum>()
        //                                .Select(e => new SelectListItem
        //                                {
        //                                    Value = e.ToString(),
        //                                    Text = e.ToString()
        //                                });
        //    // Populate Users for dropdown
        //    ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
        //    {
        //        Value = u.Id.ToString(),
        //        Text = u.Email // or any other user property you want to display
        //    }).ToList();



        //    return View(ticket);
        //}
        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketNo,Subject,TicketBody,Priority,Status,Deadline,AssignedUserId, AssignedByEmail")] Ticket ticket)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Concurrency error occurred. Another user may have edited this record.");
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Check if the exception is related to a UNIQUE constraint violation on TicketNo
                    if (ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") == true)
                    {
                        ModelState.AddModelError("TicketNo", "do not use different ticket number");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while updating the ticket. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }
            ViewBag.LeadOptions = _context.Users
             .Where(u => u.Role != "Admin" && u.Role != "User") // Adjust based on your role management
             .Select(u => new SelectListItem
             {
                 Value = u.Id.ToString(),
                 Text = u.Email // or any other user property you want to display
             })
             .ToList();
            // Re-populate Status options if validation fails or an exception occurs
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            ViewBag.UserOptions = _context.Users
               .Where(u => u.Role != "Admin" && u.Role != "TeamLead") // Adjust based on your role management
               .Select(u => new SelectListItem
               {
                   Value = u.Id.ToString(),
                   Text = u.Email // or any other user property you want to display
               })
               .ToList();

            return View(ticket);
        }


        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        // GET: Ticket/Status
        public IActionResult Status()
        {
            return View();
        }

        // GET: Ticket/GetStatusData
        public JsonResult GetStatusData()
        {

            var statusCounts = _context.Tickets
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            return Json(statusCounts);
        }
        //[HttpPost]
        //public async Task<IActionResult> SetDeadlines(List<int> ticketIds, int days)
        //{
        //    var tickets = await _context.Tickets
        //        .Where(t => ticketIds.Contains(t.Id))
        //        .ToListAsync();

        //    foreach (var ticket in tickets)
        //    {
        //        ticket.Deadline = DateTime.Now.AddDays(days);
        //    }

        //    _context.Tickets.UpdateRange(tickets);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index"); // Or another view
        //}
        //public IActionResult SetDeadlinesView()
        //{
        //    var tickets = _context.Tickets.ToList();
        //    return View("SetDeadlines", tickets);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {

            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            var tickets = await _context.Tickets
                .Select(t => new
                {
                    //t.TicketNo,
                    t.Id,
                    t.Subject,
                    t.TicketBody,
                    t.CreatedAt,
                    t.Priority,
                    t.Status
                })
                .ToListAsync();

            return Json(tickets);
        }

        [HttpGet]
        public IActionResult GetTicketsByStatus(string status)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            // Convert the status from string to StatusEnum
            if (!Enum.TryParse(status, out StatusEnum statusEnum))
            {
                // If parsing fails, return an empty list or handle the error as needed
                return Json(new List<object>());
            }

            var tickets = _context.Tickets
                .Where(t => t.Status == statusEnum) // Use the enum for comparison
                .Select(t => new
                {
                    //TicketNo = t.TicketNo,
                    Id = t.Id,
                    Subject = t.Subject,
                    TicketBody = t.TicketBody,
                    CreatedAt = t.CreatedAt,
                    Priority = t.Priority,
                    Status = t.Status
                })
                .ToList();

            return Json(tickets);
        }
        // GET: Ticket/Update/5
        public async Task<IActionResult> Update(int? id)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });
            // Populate Users for dropdown
            ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email // or any other user property you want to display
            }).ToList();

            ViewBag.LeadOptions = _context.Users
           .Where(u => u.Role != "Admin" && u.Role != "User") // Adjust based on your role management
           .Select(u => new SelectListItem
           {
               Value = u.Id.ToString(),
               Text = u.Email // or any other user property you want to display
           })
           .ToList();

            ViewBag.UserOptions = _context.Users
              .Where(u => u.Role != "Admin" && u.Role != "TeamLead") // Adjust based on your role management
              .Select(u => new SelectListItem
              {
                  Value = u.Id.ToString(),
                  Text = u.Email // or any other user property you want to display
              })
              .ToList();
            return View(ticket);
        }

        // POST: Ticket/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,TicketNo,Subject,TicketBody,Priority,Status,Deadline,AssignedUserId,AssignedByEmail")] Ticket ticket)
        {

            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("UserDashboard", "Home", new { Id = ticket.AssignedUserId });
            }

            // Re-populate Status options if validation fails
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });
            // Populate Users for dropdown
            ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email // or any other user property you want to display
            }).ToList();

            ViewBag.LeadOptions = _context.Users
         .Where(u => u.Role != "Admin" && u.Role != "User") // Adjust based on your role management
         .Select(u => new SelectListItem
         {
             Value = u.Id.ToString(),
             Text = u.Email // or any other user property you want to display
         })
         .ToList();

            ViewBag.UserOptions = _context.Users
              .Where(u => u.Role != "Admin" && u.Role != "TeamLead") // Adjust based on your role management
              .Select(u => new SelectListItem
              {
                  Value = u.Id.ToString(),
                  Text = u.Email // or any other user property you want to display
              })
              .ToList();

            return View(ticket);
        }
    }
}
