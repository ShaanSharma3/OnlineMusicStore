using System.ComponentModel.DataAnnotations;

namespace OnlineMusicStore.Models
{
    public class Music
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Singer { get; set; }

        [Required]
        public string Album { get; set; }

        [Required]
        public string Category { get; set; } // Classical, Rock, etc.

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; } // Image of CD/Cassette

        public string MusicUrl { get; set; } // URL to play preview
    }
}
