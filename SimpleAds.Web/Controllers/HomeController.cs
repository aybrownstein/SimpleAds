using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAds.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SimpleAds.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SimpleAds.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
            "Data Source=.\\sqlexpress;Initial Catalog=SimpleAd;Integrated Security=true;";


        public IActionResult Index()
        {
            var db = new AdDb(_connectionString);
            var currentUserId = GetCurrentUserId();
            List<Ad> ads = db.GetAdds();
            HomeViewModel vm = new HomeViewModel
            {
             GetAds = ads.Select(ad => new AdViewModel{
                    Ad = ad,
                    CanDelete = currentUserId != null && ad.UserId == currentUserId
                }).ToList()
            };
            return View(vm);
        }

        public IActionResult LogIn()
        {
            if(TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            var db = new AdDb(_connectionString);
            var user = db.LogIn(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid email/password";
                return Redirect("/Home/LogIn");
            }

            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "cookies", "user", "role"))).Wait();

            return Redirect("/");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var db = new AdDb(_connectionString);
            db.AddUser(user, password);
            return Redirect("/");
        }

        [Authorize]
        public IActionResult NewAd() {
            return View();
        }
   
         
        [Authorize]
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var userId = GetCurrentUserId();
            ad.UserId = userId.Value;
            AdDb db = new AdDb(_connectionString);
            db.NewAd(ad);
            return Redirect("/");
        }

        private int? GetCurrentUserId()
        {
            var db = new AdDb(_connectionString);
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            var user = db.GetByEmail(User.Identity.Name);
            if(user == null)
            {
                return null;
            }
            return user.Id;
        }

        public IActionResult Logout()
        {
            LogoutViewModel vm = new LogoutViewModel();
            HttpContext.SignOutAsync().Wait();
            var email = User.Identity.Name;
            var db = new AdDb(_connectionString);
            vm.CurrentUser = db.GetByEmail(email); 
            return View(vm);
        }

        public IActionResult Delete(int id) {
            var db = new AdDb(_connectionString);
            var userId = db.GetUserId(id);
            var currentUserId = GetCurrentUserId().Value;
        if(userId == currentUserId)
            {
                db.Delete(id);
            }
            return Redirect("/");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var db = new AdDb(_connectionString);
            int userId = GetCurrentUserId().Value;
            return View(db.GetAddsForUser(userId));
        }

    }
}
