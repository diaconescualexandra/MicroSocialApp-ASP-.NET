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
    public class FollowListsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public FollowListsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string id)
        {
            var follow = db.FollowLists
                    .Include("User")
                    .Where(u => u.UserX == id)
                    .First();

            ViewBag.FollowLists = follow;
            return View();
        }

        public IActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Where(u => u.Id == id)
                              .First();
            return View(user);
        }

    }
}
