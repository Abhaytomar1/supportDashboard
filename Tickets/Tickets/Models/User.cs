using System.ComponentModel.DataAnnotations;

namespace YourNamespace.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

     
        [Required]
        public string Role { get; set; }
    }
}
