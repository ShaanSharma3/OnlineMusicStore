using OnlineMusicStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace OnlineMusicStore.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreatedAt = DateTime.Now;

                // Save the comment to the database
                db.Comments.Add(comment);
                db.SaveChanges();

                // Show success message
                TempData["SuccessMessage"] = "Comment submitted successfully!";
                return RedirectToAction("Create"); // Redirect back to the form after submission
            }

            return View(comment); // Return the form with validation errors
        }


        public ActionResult Index()
        {
            // Fetch all comments from the database
            var comments = db.Comments.ToList();

            // Pass the list of comments to the view
            return View(comments);
        }
    }
}
