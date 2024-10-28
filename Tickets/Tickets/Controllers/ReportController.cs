using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Tickets.Models; // Your DTO or model
// Assuming you have a service interface ITicketService

namespace Tickets.Controllers
{
    public class ReportController : Controller
    {
        private readonly ITicketService _ticketService;

        public ReportController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // Display the report generation form
        public IActionResult GenerateReport()
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public IActionResult GenerateReport(DateTime startDate, DateTime endDate)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (endDate < startDate)
            {
                ModelState.AddModelError("", "End date must be after start date.");
                ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
                return View();
            }

            var tickets = _ticketService.GetTicketsByDateRange(startDate, endDate);

            // Pass the date range to the view using ViewBag
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

            return View("GenerateReport", tickets);
        }

        public IActionResult ExportReport(DateTime startDate, DateTime endDate)
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (endDate < startDate)
            {
                return BadRequest("End date must be after start date.");
            }

            var tickets = _ticketService.GetTicketsByDateRange(startDate, endDate);
            var csv = GenerateCsv(tickets);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "tickets_report.csv");
        }

        private string GenerateCsv(IEnumerable<TicketReportDto> tickets)
        {
            if (tickets == null)
            {
                return string.Empty; // Return an empty CSV if no data
            }

            var csv = new StringBuilder();
            csv.AppendLine("Id,Subject,TicketBody,CreatedAt,Priority,Status,AssignedByEmail,AssignedUserEmail");

            foreach (var ticket in tickets)
            {
                var id = EscapeCsvField(ticket.Id.ToString());
                var subject = EscapeCsvField(ticket.Subject ?? string.Empty);
                var body = EscapeCsvField(ticket.TicketBody ?? string.Empty);
                var createdAt = EscapeCsvField(ticket.CreatedAt.ToString("yyyy-MM-dd"));
                var priority = EscapeCsvField(ticket.Priority.ToString());
                var status = EscapeCsvField(ticket.Status.ToString());
                //var assignedUserId = EscapeCsvField(ticket.AssignedUserId.ToString() );
                var assignedByEmail = EscapeCsvField(ticket.AssignedByEmail);
                var assignedUserEmail = EscapeCsvField(ticket.AssignedUserEmail);

                csv.AppendLine($"{id},{subject},{body},{createdAt},{priority},{status},{assignedByEmail},{assignedUserEmail}");
            }

            return csv.ToString();
        }


        private string EscapeCsvField(string field)
        {
            if (field.Contains("\""))
            {
                field = field.Replace("\"", "\"\"");
            }
            if (field.Contains(",") || field.Contains("\n") || field.Contains("\""))
            {
                field = $"\"{field}\"";
            }
            return field;
        }
    }
}
