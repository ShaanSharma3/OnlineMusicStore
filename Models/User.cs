using System.ComponentModel.DataAnnotations;

namespace OnlineMusicStore.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Address { get; set; }

        public string Role { get; set; } = "User"; // or "Admin"
    }
}
