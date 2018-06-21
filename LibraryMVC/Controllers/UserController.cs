using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Bead1.ViewModels;
using Library.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bead1.Controllers
{
    //Type for user account management
    public class UserController : Controller
    {
        private LibraryContext _context;

        public UserController(LibraryContext context) { _context = context; }

        //Login page
        [HttpGet]
        public IActionResult Index()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        //Login specific user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel model)
        {

            if (TryValidateModel(model))
            {
                byte[] passwordBytes = null;
                using (SHA512CryptoServiceProvider provider = new SHA512CryptoServiceProvider())
                {
                    passwordBytes = provider.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                }

                //Checking if user exists + entered the good pw
                var userInDb = _context.Visitors.FirstOrDefault(v => v.Name.Equals(model.Name) && v.Password.Equals(passwordBytes));

                if(userInDb == null)
                {
                    //Error in user input
                    model.ValidationMessage = "Hibás felhasználónév, vagy jelszó!";

                    return View(model);
                }
                else
                {
                    //Login the user
                    HttpContext.Session.SetString("user", model.Name);
                }
            }
            else
            {
                return View(model);
            }

            //Redirect to main page
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            //If a user was logged in, then logout
            if (HttpContext.Session.GetString("user") != null) 
                HttpContext.Session.Remove("user");

            return RedirectToAction("Index", "Home"); // átirányítjuk a főoldalra
        }

        //Registration page
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();

            return View(model);
        }

        //Register new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if(TryValidateModel(model))
            {
                //if the given UserName's not unique
                var userInDb = _context.Visitors.FirstOrDefault(v => v.Name.Equals(model.Name));

                if(userInDb != null)
                {
                    model.ValidationMessage = "Már regisztrált ez a felhasználónév.";

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }


            //Save new user
            byte[] passwordBytes = null;
            using (SHA512CryptoServiceProvider provider = new SHA512CryptoServiceProvider())
            {
                passwordBytes = provider.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            }

            var newUser = new Visitor
            {
                Address = model.Address,
                Name = model.Name,
                Password = passwordBytes,
                PhoneNumber = model.PhoneNumber
            };

            //save new user into database
            _context.Visitors.Add(newUser);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException /*ex*/)
            {
                ModelState.AddModelError("", "Nem sikerült." + "Próbálja újra, ha továbbra sem megy " + "keresse a rendszergazdát");
            }

            //Redirect to Login page
            return RedirectToAction("Index", "User");
        }
    }
}