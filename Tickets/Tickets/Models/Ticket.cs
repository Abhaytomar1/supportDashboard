using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tickets.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        //public string TicketNo { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

       
        public string TicketBody { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public string Priority { get; set; } // Add this field

        public StatusEnum Status { get; set; } // Use the enum here

        //public DateTime? Deadline { get; set; } // New field for deadline
        [Required(ErrorMessage = "Please assign a user.")]
        public int? AssignedUserId { get; set; }

        
        // New field to track who assigned the ticket using email
        public string AssignedByEmail { get; set; } // Store email address

        

    }
}
