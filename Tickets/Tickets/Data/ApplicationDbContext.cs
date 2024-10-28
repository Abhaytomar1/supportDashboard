using Microsoft.EntityFrameworkCore;
using Tickets.Models;
using YourNamespace.Models;

namespace YourNamespace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketReportDto> TicketReportDtos { get; set; }


    }
}