using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ApplicationDbContext context,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ICategoryRepository categoryRepository)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = environment;
            _categoryRepository = categoryRepository;
        }

        //Conditii de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.IsModerator = User.IsInRole("Moderator");

            ViewBag.IsAdmin = User.IsInRole("Admin");

            ViewBag.CurrentUser = _userManager.GetUserId(User);

            // verificam daca are profilul complet
            bool userProfilComplet = false;

            if (_userManager.GetUserId(User) != null)
            {
                // userConectat = true;
                if (db.ApplicationUsers.Find(_userManager.GetUserId(User)).FirstName != null)
                    userProfilComplet = true;
            }
            
            ViewBag.UserProfilComplet = userProfilComplet;
        }


        public IActionResult Index()
        {
            SetAccessRights();

            // transmitem mesajele primite in view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.MessageType = TempData["messageType"];
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

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult New()
        {
            //transmitem mesajele primite in view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.MessageType = TempData["messageType"];
            }

            // noua categorie
            Category cat = new Category();

            return View(cat);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> New(Category cat, IFormFile file)
        {
            SetAccessRights();

            cat.UserId = _userManager.GetUserId(User);

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
                TempData["messageType"] = "alert-success";

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

        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Edit(int id)
        {
            //transmitem mesajele primite in view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.MessageType = TempData["messageType"];
            }

            // preluam categoria cautata
            Category category = db.Categories.Find(id);

            //restrictionam permisiunile
            if (category.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(category);
            }
            else
            {
                TempData["message"] = "You're unable to modify a category you didn't add!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin,Moderator")]
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
                if (category.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
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
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You're unable to modify a category you didn't add!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
           
            }
            else
            {
                return View(requestCategory);
            }
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // preluam categoria care trebuie stearsa
            Category category = db.Categories.Include("Exercises")
                                             .Where(c => c.Id == id)
                                             .First();

            if(category.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                // stergem imaginea logo din folder-ul imgs
                string path = Path.Join(_env.WebRootPath, category.Logo.Replace('/', '\\'));
                System.IO.File.Delete(path);

                // stergem categoria din baza de date
                db.Categories.Remove(category);

                // commit
                db.SaveChanges();

                TempData["message"] = "The category has been deleted";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            } 
            else
            {
                TempData["message"] = "You're unable to delete a category you didn't add!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            
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

        public IActionResult GetAllCategories()
        {
            var categories = _categoryRepository.GetAllCategories();
            return View(categories);
        }

        public IActionResult GetCategoryById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

    }
}
