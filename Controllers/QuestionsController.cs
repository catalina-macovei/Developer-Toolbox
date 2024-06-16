using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IQuestionRepository _questionRepository;

        public QuestionsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IQuestionRepository questionRepository)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _questionRepository = questionRepository;
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;


            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        public IActionResult Index()
        {
            SetAccessRights();
            var questions = db.Questions.Include("User");

            //ne definim o variabila care va stoca intrebarile impreuna cu informatiile asociate acestora necesare viw-ului
            var questionsWithAutor = from question in db.Questions.Include("User")
                                        .Select(qst => new
                                        {
                                            Question = qst,
                                            AutorFirstName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).FirstName,
                                            AutorLastName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).LastName,
                                            AutorId = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).Id,
                                            Tags = db.QuestionTags.Where(qt => qt.QuestionId == qst.Id).Select(qt => qt.Tag).ToList()
                                        })
                                     orderby question.Question.CreatedDate descending
                                     select question;
            ViewBag.QuestionsWithAutor = questionsWithAutor;
            ViewBag.Questions = questions;


            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare intrebare(dupa titlu si descriere)
                questions = db.Questions
                    .Where(q => q.Title.Contains(search) || q.Description.Contains(search))
                    .Include("User");

                questionsWithAutor = from question in questions
                                .Select(qst => new
                                {
                                    Question = qst,
                                    AutorFirstName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).FirstName,
                                    AutorLastName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).LastName,
                                    //AutorUserName = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).UserName,
                                    AutorId = db.ApplicationUsers.FirstOrDefault(user => user.Id == qst.UserId).Id,
                                    //Adaugare tags pt views
                                    Tags = db.QuestionTags.Where(qt => qt.QuestionId == qst.Id).Select(qt => qt.Tag).ToList()
                                })
                                     orderby question.Question.CreatedDate descending
                                     select question;
            }

            ViewBag.Questions = questions;
            ViewBag.QuestionsWithAutor = questionsWithAutor;
            ViewBag.SearchString = search;



            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            // Fiind un numar variabil de intrebari, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = questionsWithAutor.Count();


            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Questions/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de intrebari care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau intrebarile corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedQuestions = questionsWithAutor.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem intrebarile cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.QuestionsWithAutor = paginatedQuestions;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Questions/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Questions/Index/?page";
            }


            ///verificam daca userul este conectat, iar daca da, daca are profilul complet
            bool userProfilComplet = false;
            bool userConectat = false;
            if (_userManager.GetUserId(User) != null)
            {
                userConectat = true;
                if (db.ApplicationUsers.FirstOrDefault(user => user.Id == _userManager.GetUserId(User)).FirstName != null)
                    userProfilComplet = true;
            }

            ViewBag.UserProfilComplet = userProfilComplet;


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            ViewBag.Tags = db.Tags.ToList();

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
                                            Answer = answ,
                                            AutorFirstName = db.ApplicationUsers.FirstOrDefault(user => user.Id == answ.UserId).FirstName,
                                            AutorLastName = db.ApplicationUsers.FirstOrDefault(user => user.Id == answ.UserId).LastName,
                                            AutorId = db.ApplicationUsers.FirstOrDefault(user => user.Id == answ.UserId).Id
                                        })
                                        .ToList();
            ViewBag.QuestionWithAnswers = questionWithAnswers;

            ViewBag.Question = question;

            bool saved = false;
            string userCurent = _userManager.GetUserId(User);

            if (db.Bookmarks.Any(b => b.UserId == userCurent && b.QuestionId == id))
                saved = true;
            ViewBag.Saved = saved;

            var liked = false;
            var disliked = false;
            if (db.Reactions.Any(r => r.UserId == userCurent && r.QuestionId == question.Id))
            {
                Reaction reaction = db.Reactions.Where(r => r.UserId == userCurent && r.QuestionId == question.Id).FirstOrDefault();
                if (reaction != null)
                {
                    if (reaction.Liked == false || reaction.Liked == null)
                        liked = false;
                    else
                        liked = true;
                    if (reaction.Disliked == false || reaction.Disliked == null)
                        disliked = false;
                    else
                        disliked = true;
                }

            }
            ViewBag.Disliked = disliked;
            ViewBag.Liked = liked;

            ApplicationUser user = db.ApplicationUsers.Where(user => user.Id == question.UserId).FirstOrDefault();
            ViewBag.User = user;
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

            ApplicationUser user = db.ApplicationUsers.Where(user => user.Id == _userManager.GetUserId(User)).FirstOrDefault();

            if (user == null)
                return Redirect("/ApplicationUser/New");

            Console.BackgroundColor = ConsoleColor.Green;
            return View();
        }

        // Se adauga intrebarea in baza de date
        [HttpPost]
        public IActionResult New(Question question, List<int> TagIds)
        {
            question.UserId = _userManager.GetUserId(User);

            try
            {
                // Other properties assignment
                question.CreatedDate = DateTime.Now;
                question.DislikesNr = 0;
                question.LikesNr = 0;

                // Initialize QuestionTags collection if necessary
                if (question.QuestionTags == null)
                {
                    question.QuestionTags = new List<QuestionTag>();
                }
                // Handle tags
                foreach (var tagId in TagIds)
                {
                    var tag = db.Tags.Find(tagId);
                    if (tag != null)
                    {
                        question.QuestionTags.Add(new QuestionTag { TagId = tagId });
                    }
                }

                db.Questions.Add(question);
                db.SaveChanges();

                TempData["message"] = "The question has been successfully added.";
                TempData["messageType"] = "alert-primary";

                var refererUrl = Request.Headers["Referer"].ToString();
                return Redirect(refererUrl);
            }
            catch (Exception)
            {
                if (question.Description == null)
                    TempData["message"] = "The description field is required!";
                if (question.Title == null)
                    TempData["message"] = "The title field is required!";
                if (question.Title.Length > 100)
                    TempData["message"] = "The title cannot exceed 100 characters";
                if (question.Title.Length < 5)
                    TempData["message"] = "The title must be at least 5 characters long";
                // Exception handling code
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

            // Remove associated tags (assuming QuestionTag relationship)
            var questionTags = db.QuestionTags.Where(qt => qt.QuestionId == id);
            db.QuestionTags.RemoveRange(questionTags);

            db.Questions.Remove(question);
            TempData["message"] = "The question has been successfully deleted.";
            TempData["messageType"] = "alert-primary";

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Noua metodă GetAllQuestions
        public IActionResult GetAllQuestions()
        {
            var questions = _questionRepository.GetAllQuestions();
            return View(questions);
        }

        // Noua metodă GetQuestionById
        public IActionResult GetQuestionById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

    }
}