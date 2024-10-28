namespace Tickets.Models
{
    public class TicketPriorityViewModel
    {
        public int HighPriorityCount { get; set; }
        public int MediumPriorityCount { get; set; }
        public int LowPriorityCount { get; set; }

        public IEnumerable<Ticket> HighPriorityTickets { get; set; }
        public IEnumerable<Ticket> MediumPriorityTickets { get; set; }
        public IEnumerable<Ticket> LowPriorityTickets { get; set; }

        // Add this property
        // Combine all tickets into one list for use in the view
        public IEnumerable<Ticket> AllTickets => HighPriorityTickets
            .Concat(MediumPriorityTickets)
            .Concat(LowPriorityTickets);


    }
}
