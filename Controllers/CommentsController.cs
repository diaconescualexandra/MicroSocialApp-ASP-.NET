using proiectasp.Data;
using proiectasp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace proiectasp.Controllers
{
    public class CommentsController : Controller

    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }
            else
            {
                TempData["message"] = "Nu puteti sterge comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }

        }

        [Authorize(Roles = "User")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User))
            {
                return View(comm);
            }

            else
            {
                TempData["message"] = "Nu puteti edita comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User))
            {
                if (ModelState.IsValid)
                {
                    comm.CommentContent = requestComment.CommentContent;

                    db.SaveChanges();

                    return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu puteti edita comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }

        }


    }

}

