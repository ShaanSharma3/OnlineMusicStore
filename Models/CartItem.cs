using OnlineMusicStore.Models;
using System;


namespace OnlineMusicStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int MusicItemId { get; set; }

        public int Quantity { get; set; }

        public virtual MusicItem MusicItem { get; set; }
    }
}
