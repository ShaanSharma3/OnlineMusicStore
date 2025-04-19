using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineMusicStore.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
