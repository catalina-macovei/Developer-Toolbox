using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using static Humanizer.On;
using static System.Collections.Specialized.BitVector32;

namespace Developer_Toolbox.Controllers
{
    public class QuestionsController : Controller
    {

        private readonly ApplicationDbContext db;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public QuestionsController(ApplicationDbContext context)
            //,
            //UserManager<ApplicationUser> userManager,
            //RoleManager<IdentityRole> roleManager)
        {
            db = context;
            //_userManager = userManager;
            //_roleManager = roleManager;
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;


            //ViewBag.EsteAdmin = User.IsInRole("Admin");

            //ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        public IActionResult Index()
        {
            SetAccessRights();
            var questions = db.Questions.Include("User");
            /*
            //ne definim o variabila care va stoca intrebarile impreuna cu informatiile asociate acestora necesare viw-ului
            var questionsWithAutor = from question in db.Questions
                                        .Select(qst => new
                                        {
                                            Question = qst,
                                            AutorFirstName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).FirstName,
                                            AutorLastName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).LastName,
                                            AutorId = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).Id,
                                        })
                                 orderby question.Question.CreatedDate descending
                                 select question;
            ViewBag.QuestionsWithAutor = questionsWithAutor;
            ViewBag.Questions = questions;
            */

            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare intrebare(dupa titlu si descriere)
                questions = db.Questions
                    .Where(q => q.Title.Contains(search) || q.Description.Contains(search))
                    .Include("User");
            }

            ViewBag.Questions = questions;
            ViewBag.SearchString = search;



            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = questions.Count();


            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedQuestions = questions.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Questions = paginatedQuestions;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Questions/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Questions/Index/?page";
            }



            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }

        public ActionResult Show(int id)
        {
            SetAccessRights();
            Question question = db.Questions.Include("User")
                        .Include("Answers")
                        .Where(question => question.Id == id)
                        .First();

            //ne definim o variabila care va stoca raspunsurile corespunzatoare intrebarii impreuna cu informatiile asociate acestora necesare viw-ului
            var questionWithAnswers = question.Answers
                                        .Select(answ => new
                                        {
                                            Answer = answ
                                            //,AutorFirstName = db.ApplicationUsers.FirstOrDefault(user => user.Id == answ.UserId).FirstName,
                                            //AutorLastName = db.ApplicationUsers.FirstOrDefault(user => user.Id == answ.UserId).LastName
                                        })
                                        .ToList();
            ViewBag.QuestionWithAnswers = questionWithAnswers;

            ViewBag.Question = question;

            bool saved = false;

            if (db.Bookmarks.Any(b => b.UserId == "066215bd-686b-4fdf-b047-ec77c473e43b" && b.QuestionId == id))
                saved = true;
            ViewBag.Saved = saved;
            //ApplicationUser user = db.ApplicationUsers.Where(user => user.Id == question.UserId).FirstOrDefault();
            //ViewBag.User = user;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }

        // formularul in care se vor completa datele unei intrebari noi

        public IActionResult New()
        {
            Question question = new Question();
            if (!User.Identity.IsAuthenticated)
            {
                // Dacă utilizatorul nu este autentificat, direcționează-l către pagina de înregistrare
                return Redirect("/Identity/Account/Login");
            }
            /*
            ApplicationUser user = db.ApplicationUsers.Where(user => user.Id == _userManager.GetUserId(User)).FirstOrDefault();
            
            if (user == null)
                return Redirect("/ApplicationUser/New");
            */
            Console.BackgroundColor = ConsoleColor.Green;
            return View();
        }

        // Se adauga intrebarea in baza de date
        [HttpPost]
        public IActionResult New(Question question)
        {
            //question.UserId = _userManager.GetUserId(User);
            try
            {
                Console.BackgroundColor = ConsoleColor.Green;
                question.CreatedDate = DateTime.Now;
                question.DislikesNr = 0;
                question.LikesNr = 0;

                db.Questions.Add(question);

                TempData["message"] = "The question has been successfully added.";
                TempData["messageType"] = "alert-primary";

                db.SaveChanges();

                var refererUrl = Request.Headers["Referer"].ToString();
                return Redirect(refererUrl);
            }
            catch (Exception)
            {
                if(question.Description == null)
                    TempData["message"] = "The description field is required!";
                if(question.Title == null)
                    TempData["message"] = "The title field is required!";
                if (question.Title.Length > 100)
                    TempData["message"] = "The title cannot exceed 100 characters";
                if (question.Title.Length < 5)
                    TempData["message"] = "The title must be at least 5 characters long";
                var refererUrl = Request.Headers["Referer"].ToString();
                return Redirect(refererUrl);
            }

        }

        public IActionResult Edit(int id)
        {
            Question question = db.Questions.Find(id);

            ViewBag.Question = question;

            return View(question);
        }

        [HttpPost]
        public ActionResult Edit(int id, Question requestQuestion)
        {
            Question question = db.Questions.Find(id);

            if (ModelState.IsValid)
            {
                question.Title = requestQuestion.Title;
                question.Description = requestQuestion.Description;

                db.SaveChanges();
                TempData["message"] = "The question has been successfully edited.";
                TempData["messageType"] = "alert-primary";

                return Redirect("/Questions/Index");
               


            }
            else
            {
                return View(question);
            }
        }

        public ActionResult Delete(int id)
        {
            Question question = db.Questions.Find(id);

            var answers = db.Answers.Where(c => c.QuestionId == id);
            db.Answers.RemoveRange(answers);

            var bookmarks = db.Bookmarks.Where(b => b.QuestionId == id);
            db.Bookmarks.RemoveRange(bookmarks);

            db.Questions.Remove(question);
            TempData["message"] = "The question has been successfully deleted.";
            TempData["messageType"] = "alert-primary";
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult LikeQuestion(int questionId)
        {
            Console.WriteLine("Nr intrebare: " + questionId);
            var question = db.Questions.Find(questionId);
            if (question != null)
            {
                question.LikesNr++; // Incrementam numărului de like-uri
                db.SaveChanges();
                return Redirect("/Questions/Show/" + questionId);// Redirecționam la pagina intrebarii
            }
            // Tratam cazul în care întrebarea nu există sau alte erori
            return NotFound();
        }
        public IActionResult DislikeQuestion(int questionId)
        {
            Console.WriteLine("Nr intrebare: " + questionId);
            var question = db.Questions.Find(questionId);
            if (question != null)
            {
                question.DislikesNr++; // Decrementam numărului de like-uri
                db.SaveChanges();
                return Redirect("/Questions/Show/" + questionId);// Redirecționam la pagina intrebarii
            }
            // Tratam cazul în care întrebarea nu există sau alte erori
            return NotFound();
        }
    }
}
