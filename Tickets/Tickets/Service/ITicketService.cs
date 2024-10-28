using System.Collections.Generic;
using Tickets.Models;

public interface ITicketService
{
    IEnumerable<TicketReportDto> GetTicketsByDateRange(DateTime startDate, DateTime endDate);

}
