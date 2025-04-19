using System;
using System.Collections.Generic;

namespace OnlineMusicStore.Models
{
    public class MusicItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AlbumName { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string MusicUrl { get; set; }

        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }

        // Add the following properties
        public bool IsChartTopper { get; set; }
        public bool IsNewRelease { get; set; }



    }
}
