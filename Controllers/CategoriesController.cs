using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Developer_Toolbox.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            db = context;
            _env = env;
        }

        public IActionResult Index()
        {
            // transmitem mesajele primite in view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            // preluam categoriile din baza de date 
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;

            // transmitem categoriile in view
            ViewBag.Categories = categories;

            return View();
        }

        public ActionResult Show(int id)
        {
            // preluam categoria ceruta
            Category category = db.Categories.Find(id);

            // o transmitem catre view
            return View(category);
        }

        public ActionResult New()
        {
            // noua categorie
            Category cat = new Category();

            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> New(Category cat, IFormFile file)
        {
            // incercam sa uploadam imaginea pentru logo
            var res = await SaveImage(file);

            if (res == null)
            {
                ModelState.AddModelError("Logo", "Please load a jpg, jpeg, png, and gif file type.");
            } 
            else
            {
               cat.Logo = res;
            }

            if (ModelState.IsValid)
            {

                //add the object received to database
                db.Categories.Add(cat);

                //commit
                db.SaveChanges();

                TempData["message"] = "The category has been added";

                return RedirectToAction("Index");
            }

            else
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        // pentru debug
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                    }
                }
                return View(cat);
            }
        }

        public ActionResult Edit(int id)
        {
            // preluam categoria cautata
            Category category = db.Categories.Find(id);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category requestCategory, IFormFile file)
        {
            // preluam categoria cautata
            Category category = db.Categories.Find(id);

            // incercam sa uploadam imaginea pentru logo
            var res = await SaveImage(file);

            if (res == null)
            {
                ModelState.AddModelError("Logo", "Please load a jpg, jpeg, png or gif file type.");
            }


            if (ModelState.IsValid)
            {
                //stergem imaginea logo anterioara din folder-ul imgs
                string path = Path.Join(_env.WebRootPath, category.Logo.Replace('/', '\\'));
                System.IO.File.Delete(path);

                // modificam informatiile
                category.CategoryName = requestCategory.CategoryName;

                if (file != null && file.Length > 0)
                {
                    category.Logo = res;

                }

                //commit
                db.SaveChanges();

                TempData["message"] = "The category has been edited";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // preluam categoria care trebuie stearsa
            Category category = db.Categories.Include("Exercises")
                                             .Where(c => c.Id == id)
                                             .First();

            // stergem imaginea logo din folder-ul imgs
            string path = Path.Join(_env.WebRootPath, category.Logo.Replace('/', '\\'));
            System.IO.File.Delete(path);

       
            // stergem categoria din baza de date
            db.Categories.Remove(category);

            // commit
            db.SaveChanges();

            TempData["message"] = "The category has been deleted";
            return RedirectToAction("Index");
        }

        private async Task<string?> SaveImage(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine("img", "categories");
            var webRootPath = _env.WebRootPath;

            var uploadsFolderPath = Path.Combine(webRootPath, uploadsFolder);

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var relativeFilePath = Path.Combine(uploadsFolder, uniqueFileName).Replace(Path.DirectorySeparatorChar, '/');
            return $"/{relativeFilePath}";
        }

    }
}
