using YourNamespace.Models;

namespace Tickets.Models
{
    public class DashboardViewModel
    {
        public int HighPriorityTickets { get; set; }
        public int MediumPriorityTickets { get; set; }
        public int LowPriorityTickets { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1); // Default to one month ago
        public DateTime EndDate { get; set; } = DateTime.Now; // Default to today

        public int? AssignedUserId { get; set; }
        public string AssignedByEmail { get; set; }
        public IEnumerable<User> AssignedUsers { get; set; } // Users who can be assigned tickets
        public IEnumerable<User> AssignedByUsers { get; set; } // Users who can assign tickets
        public List<StatusChartData> StatusChartData { get; set; }

        public List<TicketsCreatedByDate> TicketsCreatedByDate { get; set; }
    }

    public class StatusChartData
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class TicketsCreatedByDate
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
