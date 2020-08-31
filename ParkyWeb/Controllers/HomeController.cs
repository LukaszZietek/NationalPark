using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Models.View;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;
        private readonly IAccountRepository _accountRepo;

        public HomeController(ILogger<HomeController> logger, INationalParkRepository npRepo, ITrailRepository trailRepo, IAccountRepository accountRepo)
        {
            _logger = logger;
            _trailRepo = trailRepo;
            _npRepo = npRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM listOfParksAndTrails = new IndexVM()
            {
                NationalParkList = await _npRepo.GetAllAsync(SD.NationalParkApiPath,HttpContext.Session.GetString("JWToken")),
                TrailList = await _trailRepo.GetAllAsync(SD.TrailApiPath, HttpContext.Session.GetString("JWToken"))
            };

            return View(listOfParksAndTrails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login()
        {
            User obj = new User();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User userObj)
        {
            User obj = await _accountRepo.LoginAsync(SD.AccountApiPath + "authenticate", userObj);
            if (obj.Token == null)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name,userObj.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role,obj.Role));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("JWToken",obj.Token);
            TempData["alert"] = "Welcome " + obj.Username;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            User obj = new User();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User userObj)
        {
            bool result = await _accountRepo.RegisterAsync(SD.AccountApiPath+"register", userObj);
            if (result == false)
            {
                return View();
            }
            TempData["alert"] = "Registration Successful ";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout(User userObj)
        {
            await HttpContext.SignOutAsync();

            HttpContext.Session.SetString("JWToken", "");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
