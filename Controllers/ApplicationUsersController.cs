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
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Guest,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            var users = db.Users.Include("Posts").Include("Comments");
            int _perPage = 5;
            int totalItems = users.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
                offset = (currentPage - 1) * _perPage;

            var paginatedUsers = users.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Users = paginatedUsers;
            return View();
        }

        [Authorize(Roles = "User,Guest,Admin")]
        public IActionResult Show(string id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            ApplicationUser user = db.Users.Include("Posts").Include("Comments")
                              .Where(u => u.Id == id)
                              .First();
            var userposts = db.Posts.Include("User")
                .Where(p => p.UserId == id);
            int _perPage = 5;
            int totalItems = userposts.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
                offset = (currentPage - 1) * _perPage;

            var paginatedPosts = userposts.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Posts = paginatedPosts;
            SetAccessRights();
            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Include("Posts").Include("Comments")
                                     .Where(u => u.Id == id)
                                     .First();
            user.LockOpt = GetLockStatus();
            if (user.Id == _userManager.GetUserId(User))
                return View(user);
            else
            {
                TempData["message"] = "Nu puteti edita un profil care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser requestUser)
        {
            ApplicationUser user = db.Users.Find(id);
            if (ModelState.IsValid)
            {
                if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    user.UserName = requestUser.UserName;
                    user.LockStatus = requestUser.LockStatus;
                    user.Signature = requestUser.Signature;
                    TempData["message"] = "Profilul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu puteti edita un profil care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Edit", user.Id);
                }
            }
            else
            {
                requestUser.LockOpt = GetLockStatus();
                return View(requestUser);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            if (user.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["message"] = "Utilizatorul a fost sters.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu puteti sterge contul unui alt utilizator.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        public IEnumerable<SelectListItem> GetLockStatus()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "false",
                Text = "Public"
            });
            selectList.Add(new SelectListItem
            {
                Value = "true",
                Text = "Privat"
            });
            return selectList;
        }
    }
}
