using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Xunit.Sdk;
using System.Text.RegularExpressions;
using Humanizer;
using Developer_Toolbox.Repositories;

namespace Developer_Toolbox.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IExerciseRepository _exerciseRepository;

        // This is for code execution:  <summary>
        private readonly HttpClient _httpClient;

        public ExercisesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, HttpClient httpClient, IExerciseRepository exerciseRepository)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpClient = httpClient;   // variable for http request to send the code
            _exerciseRepository = exerciseRepository;
        }


        //Conditii de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.IsModerator = User.IsInRole("Moderator");

            ViewBag.IsAdmin = User.IsInRole("Admin");

            ViewBag.CurrentUser = _userManager.GetUserId(User);

            // verificam daca are profilul complet
            bool completeProfile = false;

            if (_userManager.GetUserId(User) != null)
            {
                // userConectat = true;
                if (db.ApplicationUsers.Find(_userManager.GetUserId(User)).FirstName != null)
                    completeProfile = true;
            }

            ViewBag.CompleteProfile = completeProfile;
        }

        public IActionResult Index(int id)
        {
            //transmit received message to view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.MessageType = TempData["messageType"].ToString();
            }

            SetAccessRights();

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
                ViewBag.Message = TempData["message"];
                ViewBag.MessageType = TempData["messageType"];
            }

            SetAccessRights();

            // preluam exercitiul cerut
            Exercise exercise = db.Exercises.Include("Category")
                                            .Include("User")
                                            .Where(exercise => exercise.Id == id)
                                            .First();
            @ViewBag.CurrentCode = "";

            // pentru dropdown limbaje de programare
            var cpp = new SelectListItem { Text = "C++", Value = "cpp" };
            var python = new SelectListItem { Text = "Python", Value = "python" };
            List<SelectListItem> programmingLanguagesList = new List<SelectListItem> { python, cpp };


            ViewBag.ProgrammingLanguagesList = programmingLanguagesList;
            return View(exercise);

        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(string id, string x)
        {
            Solution solution = new Solution();
            solution.SolutionCode = x;
            solution.ExerciseId = int.Parse(id);
            solution.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Solutions.Add(solution);
                db.SaveChanges();

                TempData["message"] = "Your solution has been submitted";
                TempData["messageType"] = "alert-success";

                return Redirect("/Solutions/Show/" + solution.Id);
            }
            else
            {
                Exercise ex = db.Exercises.Include("Category")
                                          .Include("User")
                                          .Include("Solutions")
                                          .Where(ex => ex.Id == solution.ExerciseId)
                                          .First();

                //trimitem in view si codul curent
                ViewBag.CurrentCode = solution.SolutionCode;
                // pentru dropdown limbaje de programare
                var cpp = new SelectListItem { Text = "C++", Value = "cpp" };
                var python = new SelectListItem { Text = "Python", Value = "python" };
                List<SelectListItem> programmingLanguagesList = new List<SelectListItem> { python, cpp };


                ViewBag.ProgrammingLanguagesList = programmingLanguagesList;

                SetAccessRights();

                return View(ex);
            }
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult New()
        {
            //transmitem mesajele primite in view
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.MessageType = TempData["messageType"];
            }

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

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult New(Exercise ex)
        {

            var sanitizer = new HtmlSanitizer();

            ex.Date = DateTime.Now;
            ex.UserId = _userManager.GetUserId(User);

            // verificam daca se respecta formatul test cases
            if (ex.TestCases != null && !HasValidStructure(ex.TestCases))
            {
                ModelState.AddModelError("TestCases", "The test cases have an invalid format!");
            }

            if (ModelState.IsValid)
            {
                //protejam de cross-scripting
                ex.Restrictions = sanitizer.Sanitize(ex.Restrictions);
                ex.Examples = sanitizer.Sanitize(ex.Examples);

                db.Exercises.Add(ex);

                db.SaveChanges();

                TempData["message"] = "The Exercise has been added";
                TempData["messageType"] = "alert-success";

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

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Edit(int id)
        {
            // preluam exercitiul din baza de date
            Exercise exercise = db.Exercises.Include("Category")
                                            .Include("User")
                                            .Where(exercise => exercise.Id == id)
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

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Exercise requestedExercise)
        {
            Exercise exercise = db.Exercises.Find(id);

            var sanitizer = new HtmlSanitizer();

            if (requestedExercise.TestCases != null && !HasValidStructure(requestedExercise.TestCases))
            {
                ModelState.AddModelError("TestCases", "The test cases have an invalid format!");
            }

            if (ModelState.IsValid)
            {
                //nu permiterm modificarea exercitiului decat de admin sau de autorul lui
                if (exercise.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    //preluam noile informatii si protejam de cross-scripting
                    exercise.Title = requestedExercise.Title;
                    exercise.Description = requestedExercise.Description;
                    exercise.Summary = requestedExercise.Summary;
                    exercise.CategoryId = requestedExercise.CategoryId;
                    exercise.Restrictions = sanitizer.Sanitize(requestedExercise.Restrictions);
                    exercise.Examples = sanitizer.Sanitize(requestedExercise.Examples);
                    exercise.Difficulty = requestedExercise.Difficulty;
                    exercise.TestCases = requestedExercise.TestCases;

                    db.SaveChanges();

                    TempData["message"] = "The exercise has been modified!";
                    TempData["messageType"] = "alert-success";
                    return Redirect("/Exercises/Index/" + requestedExercise.CategoryId);
                }
                else
                {
                    TempData["message"] = "You're unable to modify an exercise you didn't add!";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Exercises/Index/" + requestedExercise.CategoryId);
                }

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

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Exercise exercise = db.Exercises.Include("Solutions")
                                            .Where(exercise => exercise.Id == id)
                                            .First();

            // pentru a ne intoarce la pagina cu exercitiile din categoria exercitiului sters
            int? categoryId = exercise.CategoryId;

            //nu permiterm stergerea exercitiului decat de admin sau de autorul lui
            if (exercise.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Exercises.Remove(exercise);
                db.SaveChanges();
                TempData["message"] = "The exercise has been deleted!";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Exercises/Index/" + categoryId);
            }
            else
            {
                TempData["message"] = "You're unable to modify an exercise you didn't add!";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Exercises/Index/" + categoryId);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ExecuteCode(int id, string solution, string language)
        {
            Solution solution1 = new Solution
            {
                ExerciseId = id,
                SolutionCode = solution,
                UserId = _userManager.GetUserId(User)
            };

            var testCases = db.Exercises.Where(ex => ex.Id == id).First().TestCases;
            var request = new
            {
                id = id,
                solution = solution,
                lang = language,
                test_cases = testCases
            };

            var jsonRequest = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:8000/execute", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserialize the outer JSON to get the `result` string
                    var outerResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

                    if (outerResult.ContainsKey("result"))
                    {
                        // Deserialize the `result` string to get the actual result dictionary
                        var innerResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(outerResult["result"]);

                        if (innerResult.ContainsKey("score"))
                        {
                            var score = Convert.ToDouble(innerResult["score"]);
                            solution1.Score = (int?)score;
                            db.Add(solution1);
                            db.SaveChanges();

                            return Json(new { status = 200, test_results = jsonResponse, score = score });
                        }
                    }
                }
                else
                {
                    // Log the response status and content for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response from backend: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
            solution1.Score = 0;
            db.Add(solution1);
            db.SaveChanges();
            return BadRequest(new { error = "Error processing request" });
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

        [NonAction]
        private bool HasValidStructure(string test_cases)
        {
            Regex parsing = new Regex(@"Input\n([^\n]+)\nOutput\n([^\n]*)", RegexOptions.IgnoreCase);
            var test_cases_cleaned = test_cases.Replace("\r", "").Trim('\n');
            // impartim in teste
            var split_tests = Regex.Split(test_cases_cleaned, @"([\n]*)Test case #[0-9]+:([\n]*)", RegexOptions.IgnoreCase);
            foreach (var test_case in split_tests)
            {
                // pentru fiecare test verificam sa aiba input si output
                var test_case_cleaned = test_case.Replace("\r", "").Trim('\n').Trim();
                if (test_case_cleaned.Length == 0) continue;
                Match match = parsing.Match(test_case_cleaned);
                if (!match.Success)
                {
                    return false;
                }
            }

            return true;
        }

        public IActionResult GetAllExercises()
        {
            var exercises = _exerciseRepository.GetAllExercises();
            return View(exercises);
        }

        // Noua metodă GetSolutionById
        public IActionResult GetExerciseById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = _exerciseRepository.GetExerciseById(id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }
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
