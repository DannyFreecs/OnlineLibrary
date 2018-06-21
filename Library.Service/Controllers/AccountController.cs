using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Library.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Library.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private LibraryContext _context;

        //Vezérlő példányosítása
        public AccountController(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Account
        [HttpGet("login/{userName}/{userPassword}")]
        public IActionResult Login(string userName, string userPassword)
        {
            byte[] passwordBytes = null;
            using (SHA512CryptoServiceProvider provider = new SHA512CryptoServiceProvider())
            {
                passwordBytes = provider.ComputeHash(Encoding.UTF8.GetBytes(userPassword));
            }

            //Checking if user exists + entered the good pw
            var userInDb = _context.Librarians.FirstOrDefault(v => v.Name.Equals(userName) && v.Password.Equals(passwordBytes));

            if (userInDb == null)
            {
                return Forbid();
            }

            return Ok();
        }

        // GET: api/Account/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
