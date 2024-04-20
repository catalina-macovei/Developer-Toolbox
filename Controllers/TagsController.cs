using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Developer_Toolbox.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext db;

        public TagsController(ApplicationDbContext context)
        {
            db = context;
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
            Tag tag = db.Tags.Find(id);

            return View(tag);
        }

        public ActionResult New()
        {
            // build a new object of type tag
            var tag = new Tag();

            return View(tag);
        }

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

        public IActionResult Edit(int id)
        {
            Tag tag = db.Tags.Find(id);

            return View(tag);
        }

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
    }
}
