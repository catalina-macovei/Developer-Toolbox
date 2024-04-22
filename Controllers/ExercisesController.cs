using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Web;


namespace Developer_Toolbox.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext db;

        public ExercisesController (ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index(int id)
        {
            //transmit received message to view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var search = "";

            ViewBag.CategoryId = id;



            // pentru ordonare exercitii in functie de dificultate
            var SelectedDifficultyOption = "";
            List<string> difficultyOptionsList = new List<string> { "Ascending", "Descending" };

            // Transformam List<string> in List<SelectListItem>
            List<SelectListItem> selectDifficultyListItems = difficultyOptionsList.Select(option =>
                new SelectListItem { Text = option, Value = option })
                .ToList();

            ViewBag.DifficultyOptionsSelectList = selectDifficultyListItems;



            // preluam exercitiile din categoria aleasa din baza de date
            IQueryable<Exercise> exercises = db.Exercises.Where(ex => ex.CategoryId == id)
                                                         .Include("User")
                                                         .Include("Category")
                                                         .OrderByDescending(ex => ex.Date);


            // motor de cautare
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // filtram rezultatele dupa search
                exercises = exercises.Where(ex => ex.Title.Contains(search) || ex.Summary.Contains(search)); 

                // daca a fost aleasa optiunea, sortam rezultatele cautarii dupa dificultate
                if (Convert.ToString(HttpContext.Request.Query["SelectedDifficultyOption"]) != null)
                {
                    SelectedDifficultyOption = Convert.ToString(HttpContext.Request.Query["SelectedDifficultyOption"]).Trim();
                }

                if (!string.IsNullOrEmpty(SelectedDifficultyOption))
                {
                    if (SelectedDifficultyOption == "Ascending")
                    {
                        exercises = exercises.AsEnumerable().OrderBy(ex => ex.Difficulty, new DifficultyComp()).AsQueryable();
                    }
                    else if (SelectedDifficultyOption == "Descending")
                    {
                        exercises = exercises.AsEnumerable().OrderByDescending(ex => ex.Difficulty, new DifficultyComp()).AsQueryable();
                    }
                }
            }

            

            // pentru transmiterea inapoi in view
            ViewBag.SearchString = search;

            string queryString = Convert.ToString(HttpContext.Request.QueryString);

            // sterg ? din substring daca exista
            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Substring(1);
            }

            // sterg page atribute, deoarece ea este stabilita in view
            var queryParameters = HttpUtility.ParseQueryString(queryString);
            queryParameters.Remove("page");
            queryString = queryParameters.ToString();

            // dau parse la valoarea querystring in view
            ViewBag.QueryString = queryString;


            // afisare paginata
            int _perPage = 10;

            int totalItems = exercises.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedExercises = exercises.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // transmitem exercitiile in view
            ViewBag.Exercises = paginatedExercises;
                            
            return View();
        }

        public IActionResult Show(int id)
        {
            //transmitem mesajele primite in view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            // preluam exercitiul cerut
            Exercise exercise = db.Exercises.Include("Category")
                                            .Include("User")
                                            .Where(exercise => exercise.Id == id)
                                            .First();

            return View(exercise);  

        }

        public IActionResult New()
        {
            Exercise ex = new Exercise();

            // preluam categoriile posibile pentru dropdown
            ex.Categories = GetAllCategories();

            // setam dificultatile pentru dropdown
            List<string> optionsList = new List<string> { "Easy", "Intermediate", "Difficult" };

            // convertim List<string> in List<SelectListItem>
            List<SelectListItem> selectListItems = optionsList.Select(option =>
                new SelectListItem { Text = option, Value = option })
                .ToList();

            ViewBag.OptionsSelectList = selectListItems;

            return View(ex);
        }

        [HttpPost]
        public IActionResult New(Exercise ex)
        {
            //de adaugat la ex user ul care a adaugat exercitiul

            var sanitizer = new HtmlSanitizer();

            ex.Date = DateTime.Now;

            if(ModelState.IsValid)
            {
                //protejam de cross-scripting
                ex.Restrictions = sanitizer.Sanitize(ex.Restrictions);
                ex.Examples = sanitizer.Sanitize(ex.Examples);

                db.Exercises.Add(ex);

                db.SaveChanges();

                TempData["message"] = "The Exercise has been added";

                return Redirect("/Exercises/Index/" + ex.CategoryId);

            }
            else
            {
                // pentru dropdown
                ex.Categories = GetAllCategories();

                List<string> optionsList = new List<string> { "Easy", "Intermediate", "Difficult" };

                // convertim List<string> in List<SelectListItem>
                List<SelectListItem> selectListItems = optionsList.Select(option =>
                    new SelectListItem { Text = option, Value = option })
                    .ToList();

                ViewBag.OptionsSelectList = selectListItems;

                return View(ex);
            }
        }

        public IActionResult Edit(int id)
        {
            // preluam exercitiul din baza de date
            Exercise exercise = db.Exercises.Include("Category")
                                            .Include("User")
                                            .Where(exercise=>exercise.Id == id)
                                            .First();
            
            // pentru dropdown categorii
            exercise.Categories = GetAllCategories();

            // pentru dropdown dificultate
            List<string> optionsList = new List<string> { "Easy", "Intermediate", "Difficult" };

            // Convert List<string> in List<SelectListItem>
            List<SelectListItem> selectListItems = optionsList.Select(option =>
                new SelectListItem { Text = option, Value = option })
                .ToList();

            ViewBag.OptionsSelectList = selectListItems;

            return View(exercise);
        }

        [HttpPost]
        public IActionResult Edit(int id, Exercise requestedExercise)
        {
            Exercise exercise = db.Exercises.Find(id);

            var sanitizer = new HtmlSanitizer();
          

            if (ModelState.IsValid)
            {
                //preluam noile informatii si protejam de cross-scripting
                exercise.Title = requestedExercise.Title;
                exercise.Description = requestedExercise.Description;
                exercise.Summary = requestedExercise.Summary;   
                exercise.CategoryId = requestedExercise.CategoryId;
                exercise.Restrictions = sanitizer.Sanitize(requestedExercise.Restrictions);
                exercise.Examples = sanitizer.Sanitize(requestedExercise.Examples);
                exercise.Difficulty = requestedExercise.Difficulty;

                db.SaveChanges();

                TempData["message"] = "The exercise has been modified!";
                TempData["messageType"] = "alert-success";
                return Redirect("/Exercises/Index/" + requestedExercise.CategoryId);

            }
            else
            {
                // pentru dropdown categorii
                exercise.Categories = GetAllCategories();

                // pentru dropdown dificultate
                List<string> optionsList = new List<string> { "Easy", "Intermediate", "Difficult" };

                // convertim List<string> in List<SelectListItem>
                List<SelectListItem> selectListItems = optionsList.Select(option =>
                    new SelectListItem { Text = option, Value = option })
                    .ToList();

                ViewBag.OptionsSelectList = selectListItems;

                return View(exercise);
            }

           
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Exercise exercise = db.Exercises.Include("Solutions")
                                            .Where(exercise => exercise.Id == id)
                                            .First();

            // pentru a ne intoarce la pagina cu exercitiile din categoria exercitiului sters
            int? categoryId = exercise.CategoryId;

            db.Exercises.Remove(exercise);
            db.SaveChanges();

            TempData["Message"] = "The exercise has been deleted!";
            TempData["MessageType"] = "alert-danger";

            return Redirect("/Exercises/Index/" + categoryId);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // preluam categoriile disponibile pentru dropdown
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                });
            }

            return selectList;
        }


    }

    // comparator custom pentru ordonare in functie de dificultate
    public class DifficultyComp : IComparer<string?>
    {
        // pentru o ordonare usoara, transformam gradele de dificultate din string in int
        private int TranslateDifficulty(string? difficulty)
        {
            if (difficulty.ToLower().Equals("easy")) return 1;
            if (difficulty.ToLower().Equals("intermediate")) return 2;
            if (difficulty.ToLower().Equals("difficult")) return 3;
            return 0;
        }

        //abia apoi le comparam
        public int Compare(string? x, string? y)
        {
            int xint = TranslateDifficulty(x);
            int yint = TranslateDifficulty(y);

            return xint.CompareTo(yint);
        }
    }
}
