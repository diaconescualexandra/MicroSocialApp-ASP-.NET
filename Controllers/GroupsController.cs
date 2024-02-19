using proiectasp.Data;
using proiectasp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

namespace proiectasp.Controllers
{

    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();                          

            if (User.IsInRole("User"))
            {
                var groups = from grup in db.Groups.Include("User")
                               .Where(b => b.UserId == _userManager.GetUserId(User))
                               select grup;

                ViewBag.Groups = groups;
                int _perPage = 5;
                int totalItems = groups.Count();
                var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
                var offset = 0;
                if (!currentPage.Equals(0))
                    offset = (currentPage - 1) * _perPage;

                var paginatedGroups = groups.Skip(offset).Take(_perPage);

                ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
                ViewBag.Groups = paginatedGroups;

                return View();
            }
            else
            if (User.IsInRole("Admin"))
            {
                var groups = from grup in db.Groups.Include("User")
                                select grup;

                ViewBag.Groups = groups;

                int _perPage = 5;
                int totalItems = groups.Count();
                var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
                var offset = 0;
                if (!currentPage.Equals(0))
                    offset = (currentPage - 1) * _perPage;

                var paginatedGroups = groups.Skip(offset).Take(_perPage);

                ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
                ViewBag.Groups = paginatedGroups;

                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi asupra grupului";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }

        }
    

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights(); 
            if (User.IsInRole("User"))
            {
                var groups = db.Groups
                                  .Include("GroupPosts.Post")
                                  .Include("GroupPosts.Post.User")
                                  .Include("User")
                                  .Where(b => b.GroupId == id)
                                  .Where(b => b.UserId == _userManager.GetUserId(User))
                                  .FirstOrDefault();

                if (groups == null)
                {
                    TempData["message"] = "Nu aveti drepturi";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }

                var groupPosts = db.GroupPosts.Include("Post")
                    .Where(p => p.GroupId == id);

                int _perPage = 5;
                int totalItems = groupPosts.Count();
                var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
                var offset = 0;
                if (!currentPage.Equals(0))
                    offset = (currentPage - 1) * _perPage;

                var paginatedPosts = groupPosts.Skip(offset).Take(_perPage);
                ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
                ViewBag.Posts = paginatedPosts;

                return View(groups);
            }

            else
            if (User.IsInRole("Admin"))
            {
                var groups = db.Groups
                                  .Include("GroupPosts.Post")
                                  .Include("GroupPosts.Post.User")
                                  .Include("User")
                                  .Where(b => b.GroupId == id)
                                  .FirstOrDefault();


                if (groups == null)
                {
                    TempData["message"] = "Resursa cautata nu poate fi gasita";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }
                var groupPosts = db.GroupPosts.Include("Post").Where(p => p.GroupId == id);

                int _perPage = 5;
                int totalItems = groupPosts.Count();
                var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
                var offset = 0;
                if (!currentPage.Equals(0))
                    offset = (currentPage - 1) * _perPage;

                var paginatedPosts = groupPosts.Skip(offset).Take(_perPage);
                ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
                ViewBag.Posts = paginatedPosts;


                return View(groups);
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetJoinMethod()
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



        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Group group = new Group();
            group.JoinMet = GetJoinMethod();
            
            return View(group);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Group gr)
        {
            gr.UserId = _userManager.GetUserId(User);

            //sa nu existe grupuri cu aceeasi denumire
            if (db.Groups.Any(g => g.GroupName == gr.GroupName))
            {
                TempData["message"] = "Un grup cu aceasta denumire exista deja";
                return RedirectToAction("New");
            }

            if (ModelState.IsValid)
            {
                db.Groups.Add(gr);
                db.SaveChanges();
                TempData["message"] = "Grup creat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                gr.JoinMet = GetJoinMethod(); 
                return View(gr);
            }
        }

        

        [Authorize(Roles = "User")]
        public IActionResult Edit(int id)
        {
            Group group = db.Groups
                .Where(p => p.GroupId == id)
                .FirstOrDefault();

            group.JoinMet = GetJoinMethod();

            if (group.UserId == _userManager.GetUserId(User))
                return View(group);
            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati grupul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult Edit(int id, Group requestGroup)
        {
            Group group = db.Groups.Find(id);
            if (ModelState.IsValid)
            {
                if (group.UserId == _userManager.GetUserId(User))
                {
                    group.GroupName = requestGroup.GroupName;
                    group.GroupDescription = requestGroup.GroupDescription;
                    group.GroupJoinMethod = requestGroup.GroupJoinMethod;
                    TempData["message"] = "Grupul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati grupul";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {   requestGroup.JoinMet=GetJoinMethod();
                return View(requestGroup);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);

            SetAccessRights();

            if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost sters";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti grupul";
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



    }
}
