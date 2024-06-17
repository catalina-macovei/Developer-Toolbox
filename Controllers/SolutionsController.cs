using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Developer_Toolbox.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;
        private readonly ISolutionRepository _solutionRepository;
        public SolutionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ISolutionRepository solutionRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            db = context;
            _solutionRepository = solutionRepository;
        }
        public async Task<IActionResult> Index()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            // Retrieve solutions for the current user
            var solutions = db.Solutions.Include(s => s.Exercise)
                                                    .Include(s => s.User)
                                                    .Where(s => s.UserId == userId)
                                                    .OrderBy(s => s.Score)
                                                    .ToList();

            ViewBag.Solutions = solutions;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }


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


        public IActionResult Show(int id) 
        { 
            Solution solution = db.Solutions.Include("Exercise")
                                        .Where(sol => sol.Id == id)
                                        .First();

            return View(solution);
        }

        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Solution solution = db.Solutions.Find(id);
            db.Solutions.Remove(solution);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult GetAllSolutions()
        {
            var solutions = _solutionRepository.GetAllSolutions();
            return View(solutions);
        }
        public IActionResult GetSolutionById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solution = _solutionRepository.GetSolutionById(id);
            if (solution == null)
            {
                return NotFound();
            }

            return View(solution);
        }

    }
}

