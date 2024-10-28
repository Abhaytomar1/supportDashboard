namespace Tickets.Models
{
    public class TicketReportDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string TicketBody { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public int? AssignedUserId { get; set; }
        public string? AssignedUserEmail { get; set; }
        public string AssignedByEmail { get; set; }
    }
}
