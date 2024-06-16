using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Developer_Toolbox.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITagRepository _tagRepository;

        public TagsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITagRepository tagRepository)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _tagRepository = tagRepository;
        }
        public IActionResult Index()
        {
            // get tags from database
            var tags = from tag in db.Tags
                       orderby tag.Name
                       select tag;

            // initialize a list of tags to be accessed from View
            ViewBag.Tags = tags;

            return View();
        }

        public IActionResult Show(int id)
        {
            // find a tag in DB

            var taggedQuestions = db.Tags
                        .Include(tag => tag.QuestionTags)
                            .ThenInclude(qt => qt.Question)
                        .FirstOrDefault(tag => tag.Id == id);

            ViewBag.taggedQuestions = taggedQuestions;

            return View(taggedQuestions);
        }

        [Authorize(Roles = "Moderator,Admin")]
        public ActionResult New()
        {
            // build a new object of type tag
            var tag = new Tag();

            return View(tag);
        }


        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public ActionResult New(Tag tag)
        {
            if (ModelState.IsValid)
            {
                db.Tags.Add(tag);
                db.SaveChanges();
                TempData["message"] = "The tag has been added";
            }
            else
            {
                return View(tag);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Moderator,Admin")]
        public IActionResult Edit(int id)
        {
            Tag tag = db.Tags.Find(id);

            return View(tag);
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public ActionResult Edit(int id, Tag requestTag)
        {
            //find the tag object to be edited
            Tag tag = db.Tags.Find(id);

            if (ModelState.IsValid)
            {
                //change its attributes accordingly
                tag.Name = requestTag.Name;

                //commit
                db.SaveChanges();

                TempData["message"] = "The tag has been edited";

                return RedirectToAction("Index");
            }
            return View(requestTag);
        }

        [Authorize(Roles = "Moderator,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //find the tag object to be deleted
            Tag tag = db.Tags.Include("QuestionTags").Where(c => c.Id == id).First();

            //delete it from the database
            db.Tags.Remove(tag);

            //commit
            db.SaveChanges();

            TempData["message"] = "The tag has been deleted";
            return RedirectToAction("Index");
        }


        // Noua metoda GetAllTags
        public IActionResult GetAllTags()
        {
            var tags = _tagRepository.GetAllTags();
            return View(tags);
        }

        // Noua metoda GetTagById
        public IActionResult GetTagById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = _tagRepository.GetTagById(id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }
    }
}
