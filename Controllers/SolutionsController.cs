using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Mvc;

namespace Developer_Toolbox.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly ApplicationDbContext db;
        public SolutionsController(ApplicationDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            var solutions = from solution in db.Solutions
                            orderby solution.Score
                            select solution;

            ViewBag.Solutions = solutions;

            return View();
        }

        public IActionResult Show(int id) 
        { 
            Solution solution = db.Solutions.Find(id);

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
    }
}
