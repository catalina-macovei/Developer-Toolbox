using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
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

        public BookmarksController(ApplicationDbContext context)
        {
            db = context;
        }
        public IActionResult Show()
        {

            ViewBag.Questions = from bookmark in db.Bookmarks.Include("Question")
                                .Where(b => b.UserId == "066215bd-686b-4fdf-b047-ec77c473e43b")
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
            var userId = "066215bd-686b-4fdf-b047-ec77c473e43b";
            if (!db.Bookmarks.Any(b => b.UserId == userId && b.QuestionId == questionId))
            {
                // Adaugă o nouă înregistrare în tabela Bookmark
                Console.WriteLine("Acesta e id-ul preluat: " + questionId);
                var bookmark = new Bookmark { UserId = userId, QuestionId = questionId };
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
            //SetAccessRights();
            Console.WriteLine("Acesta e id-ul preluat: " + questionId);
            var bookmark = db.Bookmarks
                .Where(b => b.UserId == "066215bd-686b-4fdf-b047-ec77c473e43b" && b.QuestionId == questionId).First();
            db.Bookmarks.Remove(bookmark);
            db.SaveChanges();
            TempData["message"] = "You have removed the question from the saved questions list.";
            TempData["messageType"] = "alert-primary";
            return Redirect("/Questions/Show/" + questionId);
        }

    }
}
