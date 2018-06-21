using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Library.Persistence;

namespace Library.Service.Controllers
{
    //Könyvek lekérdezését és módosítását biztosító vezérlő
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private LibraryContext _context;

        //Vezérlő példányosítása
        public BooksController(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Könyvek lekérdezése
        // GET api/values
        [HttpGet]
        public IActionResult GetBooks()
        {
            try
            {
                return Ok(_context.Books.ToList().Select(book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ReleaseYear = book.ReleaseYear,
                    ISBN = book.ISBN,
                    Picture = book.Picture
                }));
            }
            catch
            {
                //Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //Könyv köteteinek lekérdezése
        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult GetBook(Int32 id)
        {
            try
            {
                return Ok(_context.Volumes.Where(v => v.BookId == id).ToList().Select(volume => new VolumeDTO
                {
                    Id = volume.Id,
                    BookId = volume.BookId
                }));
            }
            catch
            {
                //Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //Új könyv létrehozása
        // POST api/values
        [HttpPost]
        public IActionResult PostBook([FromBody] BookDTO bookDTO)
        {
            try
            {
                var addedBook = _context.Books.Add(new Book
                {
                    Title = bookDTO.Title,
                    Author = bookDTO.Author,
                    ReleaseYear = bookDTO.ReleaseYear,
                    ISBN = bookDTO.ISBN,
                    Picture = bookDTO.Picture
                });

                _context.SaveChanges(); //új könyv elmentése

                bookDTO.Id = addedBook.Entity.Id;

                //visszaküldjük a létrehozott épületet
                return Created(Request.GetUri() + addedBook.Entity.Id.ToString(), bookDTO);
            }
            catch
            {
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
