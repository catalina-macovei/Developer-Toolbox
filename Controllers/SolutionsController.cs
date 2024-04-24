using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Developer_Toolbox.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SolutionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Index()
        {
            var solutions = db.Solutions.Include("Exercise")
                                        .OrderBy(s => s.Score);

            ViewBag.Solutions = solutions;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [Authorize(Roles = "User,Collaborator,Admin")]
        public ActionResult New()
        {
            // Retrieve the list of exercises
            var exercises = db.Exercises.ToList();

            ViewBag.Exercises = new SelectList(exercises, "Id", "Title");

            var solution = new Solution();

            // Return the view with the Solution object and the list of exercises
            return View(solution);
        }

        [HttpPost]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public ActionResult New(Solution solution)
        {
            if (ModelState.IsValid)
            {
                // Process the form data and save the new solution object
                db.Solutions.Add(solution);
                db.SaveChanges();
                TempData["message"] = "Solution created!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            // If ModelState is not valid, return the view with validation errors
            return View(solution);
        }

        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Show(int id) 
        { 
            Solution solution = db.Solutions.Include("Exercise")
                                        .Where(sol => sol.Id == id)
                                        .First();

            return View(solution);
        }

        
        [HttpPost]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Delete(int id)
        {
            Solution solution = db.Solutions.Find(id);
            db.Solutions.Remove(solution);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
