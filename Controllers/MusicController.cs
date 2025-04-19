using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OnlineMusicStore.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace OnlineMusicStore.Controllers
{
    public class MusicController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
                // Get user's cart items
                var cartItems = db.CartItems
                    .Where(c => c.UserId == userId)
                    .Select(c => c.MusicItemId)
                    .ToList();

                // Update session count variables
                Session["WishlistCount"] = wishlistItems.Count;

                // For cart count, we need the total quantity, not just the number of unique items
                int cartCount = db.CartItems
                    .Where(c => c.UserId == userId)
                    .Sum(c => c.Quantity);
                Session["CartCount"] = cartCount;

                ViewBag.WishlistItems = wishlistItems;
                ViewBag.CartItems = cartItems;
            }
            else
            {
                // Initialize empty lists for non-authenticated users
                ViewBag.WishlistItems = new List<int>();
                ViewBag.CartItems = new List<int>();

                // Clear session for non-authenticated users
                Session["WishlistCount"] = 0;
                Session["CartCount"] = 0;
            }
        }

        // GET: Music/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            MusicItem musicItem = db.MusicItems.Find(id);
            if (musicItem == null)
                return HttpNotFound();

            return View(musicItem);
        }

        // GET: Music/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Music/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Artist,AlbumName,Category,ImageUrl,MusicUrl,ReleaseDate,IsChartTopper,Price")] MusicItem musicItem)
        {
            if (ModelState.IsValid)
            {
                db.MusicItems.Add(musicItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(musicItem);
        }

        // GET: Music/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            MusicItem musicItem = db.MusicItems.Find(id);
            if (musicItem == null)
                return HttpNotFound();

            return View(musicItem);
        }


        // POST: Music/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Artist,AlbumName,Category,ImageUrl,MusicUrl,ReleaseDate,IsChartTopper,Price")] MusicItem musicItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(musicItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(musicItem);
        }

        // GET: Music/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            MusicItem musicItem = db.MusicItems.Find(id);
            if (musicItem == null)
                return HttpNotFound();

            return View(musicItem);
        }

        // POST: Music/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MusicItem musicItem = db.MusicItems.Find(id);
            db.MusicItems.Remove(musicItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Music
        public ActionResult Index()
        {
            SetUserItemLists();
            return View(db.MusicItems.ToList());
        }


        private bool IsInWishlist(int musicItemId)
        {
            var userId = User.Identity.GetUserId();
            return db.WishlistItems.Any(w => w.UserId == userId && w.MusicItemId == musicItemId);
        }

        private bool IsInCart(int musicItemId)
        {
            var userId = User.Identity.GetUserId();
            return db.CartItems.Any(c => c.UserId == userId && c.MusicItemId == musicItemId);
        }

        // Modify the Details action to pass this information to the view
      

        // GET: Music/ChartToppers
        public ActionResult ChartToppers()
        {
            var chartToppers = db.MusicItems
                .Where(m => m.IsChartTopper)
                .OrderByDescending(m => m.ReleaseDate)
                .ToList();

            SetUserItemLists();
            return View(chartToppers);
        }

        // GET: Music/NewReleases
        public ActionResult NewReleases()
        {
            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

            var newReleases = db.MusicItems
                .Where(m => m.ReleaseDate > threeMonthsAgo)
                .OrderByDescending(m => m.ReleaseDate)
                .ToList();

            SetUserItemLists();
            return View(newReleases);
        }

        [HttpGet]
        public ActionResult AddToWishlist(int id)
        {
            var userId = User.Identity.GetUserId();
            var existing = db.WishlistItems
                             .FirstOrDefault(w => w.UserId == userId && w.MusicItemId == id);
            if (existing == null)
            {
                db.WishlistItems.Add(new WishlistItem
                {
                    UserId = userId,
                    MusicItemId = id,
                });
                db.SaveChanges();
                TempData["WishlistSuccess"] = "Item successfully added to your wishlist!";

                // Update the wishlist count in session
                int wishlistCount = 0;
                if (Session["WishlistCount"] != null)
                {
                    wishlistCount = (int)Session["WishlistCount"];
                }
                Session["WishlistCount"] = wishlistCount + 1;
            }
            else
            {
                // Change the message to indicate it's already in wishlist
                TempData["WishlistInfo"] = "This item is already in your wishlist!";
                // You could also add a link to the wishlist page
                TempData["WishlistRedirect"] = true;
            }
            return Redirect(Request.UrlReferrer?.ToString() ?? Url.Action("Index", "Home"));
        }

        // Add to Cart
        [HttpGet]
        public ActionResult AddToCart(int id)
        {
            var userId = User.Identity.GetUserId();
            var existing = db.CartItems
                             .FirstOrDefault(c => c.UserId == userId && c.MusicItemId == id);
            if (existing == null)
            {
                db.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    MusicItemId = id,
                    Quantity = 1,
                });
                db.SaveChanges();
                TempData["CartSuccess"] = "Item successfully added to your cart!";

                // Update the cart count in session
                int cartCount = 0;
                if (Session["CartCount"] != null)
                {
                    cartCount = (int)Session["CartCount"];
                }
                Session["CartCount"] = cartCount + 1;
            }
            else
            {
                // Change the message to suggest updating quantity in cart
                TempData["CartInfo"] = "This item is already in your cart. You can update quantity on the cart page.";
                // You could also add a link to the cart page
                TempData["CartRedirect"] = true;
            }
            return Redirect(Request.UrlReferrer?.ToString() ?? Url.Action("Index", "Home"));
        }

        // View Wishlist
        public ActionResult Wishlist()
        {
            var userId = User.Identity.GetUserId();

            var wishlist = db.WishlistItems
                             .Where(w => w.UserId == userId)
                             .Select(w => w.MusicItem)
                             .ToList();

            // Get cart items to check if wishlist items are already in cart
            var cartItems = db.CartItems
                             .Where(c => c.UserId == userId)
                             .Select(c => c.MusicItemId)
                             .ToList();

            ViewBag.CartItems = cartItems;

            // Update session with current wishlist count
            Session["WishlistCount"] = wishlist.Count;

            return View(wishlist);
        }

        // View Cart
        public ActionResult Cart()
        {
            var userId = User.Identity.GetUserId();

            var cart = db.CartItems
                         .Where(c => c.UserId == userId)
                         .ToList();

            // Update session with current cart quantity
            int cartCount = cart.Sum(c => c.Quantity);
            Session["CartCount"] = cartCount;

            return View(cart);
        }

        // Remove from Cart
        [HttpGet]
        public ActionResult RemoveFromCart(int id)
        {
            var userId = User.Identity.GetUserId();

            var item = db.CartItems.FirstOrDefault(c => c.UserId == userId && c.MusicItemId == id);
            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();

                // Fix: handle null case in sum
                int? cartCountNullable = db.CartItems
                    .Where(c => c.UserId == userId)
                    .Sum(c => (int?)c.Quantity);

                Session["CartCount"] = cartCountNullable ?? 0;
            }

            return RedirectToAction("Cart");
        }


        // Remove from Wishlist
        [HttpGet]
        public ActionResult RemoveFromWishlist(int id)
        {
            var userId = User.Identity.GetUserId();

            var item = db.WishlistItems.FirstOrDefault(w => w.UserId == userId && w.MusicItemId == id);
            if (item != null)
            {
                db.WishlistItems.Remove(item);
                db.SaveChanges();

                // Update wishlist count in session
                int wishlistCount = db.WishlistItems
                                   .Count(w => w.UserId == userId);
                Session["WishlistCount"] = wishlistCount;
            }

            return RedirectToAction("Wishlist");
        }

        // Update Quantity in Cart
        [HttpPost]
        //[Authorize]
        public ActionResult UpdateQuantity(int id, int newQuantity)
        {
            var userId = User.Identity.GetUserId();

            // Check if the new quantity is valid
            if (newQuantity <= 0)
            {
                // If the quantity is less than or equal to 0, remove the item from the cart
                var cartItem = db.CartItems.FirstOrDefault(c => c.UserId == userId && c.MusicItemId == id);
                if (cartItem != null)
                {
                    db.CartItems.Remove(cartItem);
                    db.SaveChanges();
                }
            }
            else
            {
                // Update the quantity if it's valid
                var cartItem = db.CartItems.FirstOrDefault(c => c.UserId == userId && c.MusicItemId == id);
                if (cartItem != null)
                {
                    cartItem.Quantity = newQuantity;
                    db.SaveChanges();
                }
            }

            // Update cart count in session
            int cartCount = db.CartItems
                          .Where(c => c.UserId == userId)
                          .Sum(c => c.Quantity);
            Session["CartCount"] = cartCount;

            return RedirectToAction("Cart");
        }

        // Buy Now - Step 1: Show Cart and Proceed to Payment Form
        public ActionResult BuyNow()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems.Where(c => c.UserId == userId).ToList();

            if (cartItems.Count == 0)
            {
                return RedirectToAction("Cart");
            }

            // Calculate total cost
            decimal totalAmount = cartItems.Sum(c => c.MusicItem.Price * c.Quantity);

            // Passing the total amount and cart items to the view
            ViewBag.TotalAmount = totalAmount;
            return View(cartItems); // This will render the checkout summary
        }

        // Payment Form - Step 2: Show Payment Form
        [HttpPost]
        public ActionResult Payment()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems.Where(c => c.UserId == userId).ToList();

            if (cartItems.Count == 0)
            {
                return RedirectToAction("Cart");
            }

            // Calculate total cost
            decimal totalAmount = cartItems.Sum(c => c.MusicItem.Price * c.Quantity);

            // Passing the total amount to the view
            ViewBag.TotalAmount = totalAmount;
            return View(cartItems); // This will render the Payment form
        }

        // Checkout - Step 3: Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Checkout(string CardNumber, string CardHolder, string ExpirationDate, string CVC)
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems.Where(c => c.UserId == userId).ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Cart");
            }

            // Validate payment details (simplified for demonstration)
            if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardHolder) ||
                string.IsNullOrEmpty(ExpirationDate) || string.IsNullOrEmpty(CVC))
            {
                // Payment details are invalid, return to payment form with error
                ViewBag.ErrorMessage = "Please fill in all payment details.";
                decimal totalAmount = cartItems.Sum(c => c.MusicItem.Price * c.Quantity);
                ViewBag.TotalAmount = totalAmount;
                return View("Payment", cartItems);
            }

            // Process payment here (for now, just clear the cart after receiving payment details)
            bool paymentSuccessful = true; // Simulate successful payment

            if (paymentSuccessful)
            {
                // Calculate total amount
                decimal totalAmount = cartItems.Sum(c => c.MusicItem.Price * c.Quantity);

                // Create new order
                var order = new Order
                {
                    UserId = (userId), // You might need to convert string userId to int
                    CardHolder = CardHolder,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Items = new List<OrderItem>()
                };

                // Add order items
                foreach (var item in cartItems)
                {
                    order.Items.Add(new OrderItem
                    {
                        MusicItemId = item.MusicItemId,
                        Quantity = item.Quantity,
                        Price = item.MusicItem.Price
                    });
                }

                // Save order to database
                db.Orders.Add(order);
                db.SaveChanges();

                // After successful payment and order creation, clear the cart
                foreach (var item in cartItems)
                {
                    db.CartItems.Remove(item);  // Clear cart after purchase
                }

                db.SaveChanges();

                // Reset cart count in session
                Session["CartCount"] = 0;

                // Redirect to the success page
                return RedirectToAction("OrderSuccess");
            }

            // If payment failed, show an error or stay on the payment page
            ViewBag.ErrorMessage = "Payment failed, please try again.";
            decimal totalAmountOnFailure = cartItems.Sum(c => c.MusicItem.Price * c.Quantity);
            ViewBag.TotalAmount = totalAmountOnFailure;

            return View("Payment", cartItems); // Stay on the payment page with cart items
        }



        // Order Success Page
        public ActionResult OrderSuccess()
        {
            return View();
        }

        public ActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index", "Home");
            }

            var searchResults = db.MusicItems
                .Where(m => m.Title.Contains(searchTerm))
                .ToList();

            // Pass both the search results and the search term to the view
            ViewBag.SearchTerm = searchTerm;

            return View(searchResults);
        }

       
        // Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}