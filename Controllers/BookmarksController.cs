using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Developer_Toolbox.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarksController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IBookmarkRepository bookmarkRepository)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _bookmarkRepository = bookmarkRepository;
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;


            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        
        public IActionResult Show()
        {
            var userCurent = _userManager.GetUserId(User);
            ViewBag.Questions = from bookmark in db.Bookmarks.Include("Question")
                                .Where(b => b.UserId == userCurent)
                                select bookmark.Question; 

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }


        // Salvarea unei întrebări în bookmark-ul utilizatorului
        public IActionResult Save(int questionId)
        {
            // Verifică dacă utilizatorul nu a salvat deja întrebarea
            var userCurent = _userManager.GetUserId(User);
            if (!db.Bookmarks.Any(b => b.UserId == userCurent && b.QuestionId == questionId))
            {
                // Adaugă o nouă înregistrare în tabela Bookmark
                Console.WriteLine("Acesta e id-ul preluat: " + questionId);
                var bookmark = new Bookmark { UserId = userCurent, QuestionId = questionId };
                TempData["message"] = "The question has been successfully saved!";
                TempData["messageType"] = "alert-primary";
                db.Bookmarks.Add(bookmark);
                db.SaveChanges();
            }

            // Redirectează la pagina cu salvări
            return Redirect("/Questions/Show/" + questionId);

        }

        // Stergerea unei întrebări din salvări

        public IActionResult Unsave(int questionId)
        {
            SetAccessRights();
            var userCurent = _userManager.GetUserId(User);
            Console.WriteLine("Acesta e id-ul preluat: " + questionId);
            var bookmark = db.Bookmarks
                .Where(b => b.UserId == userCurent && b.QuestionId == questionId).First();
            db.Bookmarks.Remove(bookmark);
            db.SaveChanges();
            TempData["message"] = "You have removed the question from the saved questions list.";
            TempData["messageType"] = "alert-primary";
            return Redirect("/Questions/Show/" + questionId);
        }


        // Noua metoda GetAllBookmarks
        public IActionResult GetAllBookmarks()
        {
            var bookmarks = _bookmarkRepository.GetAllBookmarks();
            return View(bookmarks);
        }

        // Noua metoda GetBookmarkById
        public IActionResult GetBookmarkById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookmark = _bookmarkRepository.GetBookmarkById(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            return View(bookmark);
        }

    }
}
