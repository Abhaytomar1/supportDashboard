// Service/TicketService.cs
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tickets.Models;
using YourNamespace.Data;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TicketReportDto> GetTicketsByDateRange(DateTime startDate, DateTime endDate)
    {
        var startDateParam = new SqlParameter("@StartDate", SqlDbType.Date) { Value = startDate };
        var endDateParam = new SqlParameter("@EndDate", SqlDbType.Date) { Value = endDate };

        var tickets = _context.TicketReportDtos
            .FromSqlRaw("EXEC GetTicketsByDateRange @StartDate, @EndDate", startDateParam, endDateParam)
            .ToList();

        return tickets;
    }
}
