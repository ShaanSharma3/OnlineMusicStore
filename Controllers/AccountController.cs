using System.Linq;
using System.Web.Mvc;
using OnlineMusicStore.Models;
using System.Security.Cryptography;
using System.Text;
using System;

namespace OnlineMusicStore.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Account/Register
        public ActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = HashPassword(user.PasswordHash);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: /Account/Login
        public ActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            string hashed = HashPassword(password);
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashed);

            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["UserName"] = user.FullName;
                Session["UserRole"] = user.Role;

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials!";
            return View();
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public ActionResult UserProfile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserProfileViewModel
            {
                Name = user.FullName,         // Make sure your User table has this field
                Email = user.Email,
                Address = user.Address,       // Same here
                Role = user.Role              // Make sure you have a Role column or logic
            };

            return View(model);
        }

    }
}
