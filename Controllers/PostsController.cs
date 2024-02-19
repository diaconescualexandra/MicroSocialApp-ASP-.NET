using proiectasp.Data;
using proiectasp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

//alexandra
namespace proiectasp.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IWebHostEnvironment _env;
        private ApplicationDbContext _context;


        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
           // _context = context; 


        }

        // Variabila locala de tip AppDBContext


        // In constructor, se face dependency injection

        // Alocam conexiunea (injectata) cu baza de date unei proprietati
        //locale pentru a fi refolosita in metodele controller-ului



        // Afisam view-ul cu form-ul
        //public IActionResult UploadImage()
        //{
        //    return View();
        //}


        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Post post = new Post();
            return View(post);
        }

        //[Authorize(Roles = "User,Admin")]
        //[HttpPost]
        //public IActionResult New(Post post)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Posts.Add(post);
        //        db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View(post);
        //    }
        //}


        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> New(Post post, IFormFile Image)
        {   // Verificam daca exista imaginea in request (daca a fost
            //incarcata o imagine)
            post.UserId = _userManager.GetUserId(User);

            try
            {
                string databaseFileName = null;
                if (Image.Length > 0)
                {
                    // Generam calea de stocare a fisierului
                    var storagePath = Path.Combine(
                            _env.WebRootPath, // Preluam calea folderului wwwroot
                            "images", // Adaugam calea folderului images
                            Image.FileName // Numele fisierului
                    );

                    // Generam calea de afisare a fisierului care va fi stocata in
                    // baza de date
                    databaseFileName = "/images/" + Image.FileName;
                    // Uploadam fisierul la calea de storage
                    using (var fileStream = new FileStream(storagePath,
                    FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }

                    post.Image = databaseFileName;

                }
                // Salvam storagePath-ul in baza de date
                
                post.PostCreationTime = DateTime.Now;
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postare creata.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                return RedirectToAction("New");

            }
        }


        [Authorize(Roles = "User,Guest,Admin")]
        public IActionResult Index()
        {
            var posts = db.Posts.Include("User");


            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }
            int _perPage = 5;
            int totalItems = posts.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
                offset = (currentPage - 1) * _perPage;

            var paginatedPosts = posts.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Posts = paginatedPosts;
            SetAccessRights();

            return View();
        }


        [Authorize(Roles = "User,Guest,Admin")]
        public IActionResult Show(int id)
        {


            Post post = db.Posts.Include("Comments")
                            .Include("User")
                            .Include("Comments.User")
                            .Where(u => u.PostId == id)
                            .First();

            ViewBag.UserGroups = db.Groups
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();


            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            //var comments = db.Comments.Where(c => c.PostId == id);
            //int _perPage = 5;
            //int totalItems = comments.Count();
            //var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            //var offset = 0;
            //if (!currentPage.Equals(0))
            //    offset = (currentPage - 1) * _perPage;

            //var paginatedComments = comments.Skip(offset).Take(_perPage);

            //ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            //ViewBag.Comments = paginatedComments;

            return View(post);

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.CommentCreationTime = DateTime.Now;

            // preluam id-ul utilizatorului care posteaza comentariul
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }
            else
            {
                Post post = db.Posts.Include("User")
                                    .Include("Comments")
                                    .Include("Comments.User")
                                    .Where(post => post.PostId == comment.PostId)
                                    .First();


                ViewBag.UserGroups = db.Groups
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();

                SetAccessRights();

                return View(post);
            }
        }

        [HttpPost]
        public IActionResult AddGroup([FromForm] GroupPost groupPost)
        {
            // Daca modelul este valid
            if (ModelState.IsValid)
            {
                // Verificam daca avem deja postarea in grup
                if (db.GroupPosts
                    .Where(ab => ab.PostId == groupPost.PostId)
                    .Where(ab => ab.GroupId == groupPost.GroupId)
                    .Count() > 0)
                {
                    TempData["message"] = "Aceasta postare este deja in grup";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    // Adaugam asocierea intre grup si postare 
                    db.GroupPosts.Add(groupPost);
                    // Salvam modificarile
                    db.SaveChanges();

                    // Adaugam un mesaj de succes
                    TempData["message"] = "postare a fost trimisa pe grup";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "Nu s-a putut trimite postarea pe grup";
                TempData["messageType"] = "alert-danger";
            }

            // Ne intoarcem la pagina articolului
            return Redirect("/Posts/Show/" + groupPost.PostId);
        }


        
        

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts
                            .Where(p => p.PostId == id)
                            .First();

            //Post post = db.Posts
            //            .Where(p => p.PostId == id)
            //            .FirstOrDefault();


            if (post.UserId == _userManager.GetUserId(User))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati o postare care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {
            Post post = db.Posts.Find(id);
            if (ModelState.IsValid)
            {
                if (post.UserId == _userManager.GetUserId(User))
                {
                    post.Image=requestPost.Image;
                    post.PostCaption = requestPost.PostCaption;
                    post.PostContent = requestPost.PostContent;
                    TempData["message"] = "Postarea a fost modificata";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati o postare care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestPost);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Include("Comments")
                                .Where(p => p.PostId == id)
                                .First();
            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost stearsa.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o postare care nu va apartine";
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
