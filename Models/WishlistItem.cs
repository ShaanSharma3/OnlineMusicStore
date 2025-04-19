using OnlineMusicStore.Models;

public class WishlistItem
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int MusicItemId { get; set; }

    public virtual MusicItem MusicItem { get; set; }
}
