using OnlineMusicStore.Models;
using System.Web.Mvc;
using System.Linq;
namespace OnlineMusicStore.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Hard-coded admin credentials as requested
            if (username == "admin" && password == "admin")
            {
                // Store admin session
                Session["AdminLoggedIn"] = true;
                Session["AdminUsername"] = username;
                // Get actual counts from database
                int wishlistCount = db.WishlistItems.Count();
                // Handle nullable Quantity field using null-coalescing operator
                // int cartCount = db.CartItems.Sum(c => c.Quantity);  // This should handle null Quantity as 0
                // Set counts in session
                Session["WishlistCount"] = wishlistCount;
                // Session["CartCount"] = cartCount;
                // Redirect to Music index page as requested
                return Redirect("https://localhost:44308/Music");
            }
            // Invalid login
            ViewBag.ErrorMessage = "Invalid username or password";
            return View();
        }
        // Logout
        public ActionResult Logout()
        {
            Session["AdminLoggedIn"] = null;
            Session["AdminUsername"] = null;
            // Clear counts from session
            Session["WishlistCount"] = null;
            Session["CartCount"] = null;
            return RedirectToAction("Login");
        }
        // Show all users
        public ActionResult Users()
        {
            if (Session["AdminLoggedIn"] == null || !(bool)Session["AdminLoggedIn"])
                return RedirectToAction("Login");
            var users = db.Users.Where(u => u.Role != "Admin").ToList(); // Exclude Admins
            return View(users);
        }
        // Remove a user
        [HttpPost]
        public ActionResult RemoveUser(int id)
        {
            if (Session["AdminLoggedIn"] == null || !(bool)Session["AdminLoggedIn"])
                return RedirectToAction("Login");
            var user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["SuccessMessage"] = "User removed successfully.";
            }
            return RedirectToAction("Users");
        }



        public ActionResult ViewOrders()
        {
            var orders = db.Orders.Include("Items.MusicItem").ToList();
            return View(orders);
        }


    }
}

