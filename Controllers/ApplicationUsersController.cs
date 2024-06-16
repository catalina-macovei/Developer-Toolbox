using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Developer_Toolbox.Controllers
{
    public class ApplicationUsersController : Controller
    {

       private readonly ApplicationDbContext db;
       private readonly UserManager<ApplicationUser> _userManager;
       private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;



        public ApplicationUsersController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


        // lista tututor userilor si search bar

        public IActionResult Index(string search)
        {
            SetAccessRights();
            var users = db.ApplicationUsers.Where(u => !string.IsNullOrEmpty(u.FirstName) && !string.IsNullOrEmpty(u.LastName)); // Convertim DbSet în interogare

            if (!string.IsNullOrEmpty(search))
            {
                // Cautăm după nume sau prenume
                users = users.Where(a => a.FirstName.Contains(search) || a.LastName.Contains(search));
            }

            ViewBag.Users = users.ToList(); // Obținem lista completă de utilizatori după aplicarea filtrului
            ViewBag.SearchString = search;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }



        //afisarea unui singur profil in functie de id-ul sau
        public IActionResult Show(string? id)
        {
            SetAccessRights();
            
                if (!User.Identity.IsAuthenticated)
                {
                    // Dacă utilizatorul nu este autentificat, direcționează-l către pagina de înregistrare
                    return Redirect("/Identity/Account/Login");
                }

                //ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            ApplicationUser user = db.ApplicationUsers
                          .Where(u => u.Id == id)
                          .FirstOrDefault();

               ViewBag.User = user;

            var questions = db.Questions.Include("User").Where(q => q.UserId == id);
            var answers = db.Answers.Include("User").Where(a => a.UserId == id);

            var answerCount = db.Answers.Count(a => a.UserId == id);

            var questionCount = db.Questions.Count(a => a.UserId == id);


            ViewBag.Questions = questions;
            ViewBag.Answers = answers;
            ViewBag.AnswerCount = answerCount;
            ViewBag.QuestionCount = questionCount;
            ViewBag.Scor = answerCount * 10 + questionCount * 5;


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        // formularul in care se vor completa datele unei profil nouu
        public IActionResult New()
        {
            ApplicationUser user = new ApplicationUser();

            return View(user);
        }


        // Se adauga profilul in baza de date

        [HttpPost]
        public IActionResult New(ApplicationUser user)
        {

            user.Id = _userManager.GetUserId(User);

           // if (ModelState.IsValid)
           // {

                db.ApplicationUsers.Add(user);

                //TempData["message"] = "Profilul a fost creat cu succes";
                //["messageType"] = "alert-success";
                db.SaveChanges();
            return View(user);
               // return RedirectToAction("Show");
           // }

          /*  else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View(profile);
            }
          */

        }

        // Se editeaza un profil existent in baza de date 
        // HttpGet implicit
        // Se afiseaza formularul impreuna cu datele aferente profilului din baza de date

        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ApplicationUser user = db.ApplicationUsers
                                        .Where(u => u.Id == _userManager.GetUserId(User))
                                        .First();
           
            user.Id = _userManager.GetUserId(User);
            ViewBag.User = user;
            return View(user);
        }

        // Se adauga profilul modificat in baza de date
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser requestProfile)
        {
            ApplicationUser user = db.Users.Find(id);

            // if (ModelState.IsValid)
            // {

            user.FirstName = requestProfile.FirstName;
            user.LastName = requestProfile.LastName;
            user.Description = requestProfile.Description;
            user.Birthday= requestProfile.Birthday;
           
            TempData["message"] = "You edited your profile successfully!";
            TempData["messageType"] = "alert-success";
            db.SaveChanges();
            
            return RedirectToAction("Index");
            //}
            //else
            //{
              //  return View(requestProfile);
            //}
        }
        /*

        [Authorize(Roles = "User,Admin")]*/
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = db.ApplicationUsers
                                   .Where(user => user.Id == id).First();

           

                if (user == null)
            {
                // Handle the case where user is not found
                return RedirectToAction("Index");
            }

            if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {

                db.ApplicationUsers.Remove(user);
               TempData["message"] = "Your profile has been deleted.";
               TempData["messageType"] = "alert-success";
                db.SaveChanges();

                await _signInManager.SignOutAsync(); 

                // Redirect to the Index page
                return RedirectToAction("Index");
                
            }

            else return RedirectToAction("Index");

        }
        

    }

}