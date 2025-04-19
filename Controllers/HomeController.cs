using OnlineMusicStore.Models;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
public class HomeController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    // In HomeController.cs
    // Add this helper method to your HomeController
    private void SetUserItemLists()
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.Identity.GetUserId();

            // Get user's wishlist items
            var wishlistItems = db.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.MusicItemId)
                .ToList();

            // Get user's cart items WITH quantities
            var cartItems = db.CartItems
                .Where(c => c.UserId == userId)
                .Select(c => c.MusicItemId)
                .ToList();

            // Get cart quantities as a dictionary of MusicItemId -> Quantity
            var cartQuantities = db.CartItems
                .Where(c => c.UserId == userId)
                .ToDictionary(c => c.MusicItemId, c => c.Quantity);

            ViewBag.WishlistItems = wishlistItems;
            ViewBag.CartItems = cartItems;
            ViewBag.CartQuantities = cartQuantities;
        }
        else
        {
            // Initialize empty collections for non-authenticated users
            ViewBag.WishlistItems = new List<int>();
            ViewBag.CartItems = new List<int>();
            ViewBag.CartQuantities = new Dictionary<int, int>();
        }
    }

    // Then update each action method to call this helper
    public ActionResult Index()
    {
        var groupedMusic = db.MusicItems
            .GroupBy(m => m.Category)
            .ToDictionary(g => g.Key, g => g.ToList());

        SetUserItemLists();

        return View(groupedMusic);
    }

    public ActionResult NewReleases()
    {
        // Get the latest 6 music items ordered by ReleaseDate
        var newReleases = db.MusicItems
                            .OrderByDescending(m => m.ReleaseDate)
                            .Take(6)
                            .ToList();

        SetUserItemLists();

        return View(newReleases);
    }

    public ActionResult ChartToppers()
    {
        // Exclude New Releases from Chart Toppers
        var newReleases = db.MusicItems
                             .OrderByDescending(m => m.ReleaseDate)
                             .Take(6)
                             .Select(m => m.Id)
                             .ToList();
        // Get the chart toppers by excluding new releases
        var chartToppers = db.MusicItems
                             .Where(m => !newReleases.Contains(m.Id))
                             .OrderByDescending(m => m.Id)  // Or use another metric like votes or play count
                             .Take(6)
                             .ToList();

        SetUserItemLists();

        return View(chartToppers);
    }

    public ActionResult Help()
    {
        return View();
    }

    public ActionResult About()
    {
        return View();
    }
}
