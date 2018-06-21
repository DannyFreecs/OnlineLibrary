using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Bead1.ViewModels;
using System.IO;
using Library.Persistence;

namespace Bead1.Controllers
{
    //Main Controller
    public class HomeController : Controller
    {
        private LibraryContext _context;

        //Constructor
        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        /// <summary>
		/// Egy akció meghívása után végrehajtandó metódus.
		/// </summary>
		/// <param name="context">Az akció kontextus argumentuma.</param>
		public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // Booklist
            ViewBag.Books = _context.Books.ToArray();
        }

        //Main page action
        [HttpGet]
        public IActionResult Index(string sortOrder, string currentTitle, string currentAuthor, string searchTitle, string searchAuthor, int? page)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if ((searchTitle != null) || (searchAuthor != null))
            {
                page = 1;
            }
            if (searchTitle == null)
            {
                searchTitle = currentTitle;
            }
            if (searchAuthor == null)
            {
                searchAuthor = currentAuthor;
            }

            ViewData["TitleFilter"] = searchTitle;
            ViewData["AuthorFilter"] = searchAuthor;

            List<Book> books = new List<Book>();

            //Query books by popularity
            var conn = _context.Database.GetDbConnection();
            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    string query = "SELECT b.Id, b.Author, b.ISBN, b.Picture, b.ReleaseYear, b.Title "
                        + "FROM Books b "
                        + "INNER JOIN Volumes v on b.Id=v.BookId "
                        + "LEFT OUTER JOIN Rents r on v.Id=r.VolumeId "
                        + "GROUP BY b.Id, b.Author, b.ISBN, b.Picture, b.ReleaseYear, b.Title "
                        + "ORDER BY COUNT(r.VolumeId) desc, b.Title";
                    command.CommandText = query;
                    DbDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = new Book();
                            row.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            row.Author = reader.GetString(reader.GetOrdinal("Author"));
                            row.ISBN = reader.GetString(reader.GetOrdinal("ISBN"));
                            row.ReleaseYear = reader.GetInt32(reader.GetOrdinal("ReleaseYear"));
                            row.Title = reader.GetString(reader.GetOrdinal("Title"));

                            //Buffer for picture
                            byte[] picData = new byte[150000];

                            MemoryStream ms = new MemoryStream();
                            int index = 0;
                            while (true)
                            {
                                long count = reader.GetBytes(reader.GetOrdinal("Picture"),
                                   index, picData, 0, picData.Length);
                                if (count == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    index = index + (int)count;
                                    ms.Write(picData, 0, (int)count);
                                }
                            }
                            row.Picture = ms.ToArray();

                            books.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }

            //Searching by Title && Author
            if (!String.IsNullOrEmpty(searchTitle))
            {
                books = books.Where(b => b.Title.ToUpper().Contains(searchTitle.ToUpper())).ToList();
            }
            if (!String.IsNullOrEmpty(searchAuthor))
            {
                books = books.Where(b => b.Author.ToUpper().Contains(searchAuthor.ToUpper())).ToList();
            }

            //Sorting by given logic
            switch (sortOrder)
            {
                case "name_desc":
                    books = books.OrderBy(b => b.Title).ToList();
                    break;
                default:
                    break;
            }

            //Max number of books/page
            int pageSize = 20;

            return View("Index", PaginatedList<Book>.Create(books, page ?? 1, pageSize));
        }


        //Get specific book and the corresponding data
        [HttpGet]
        public IActionResult Details(Int32 bookId)
        {
            Book book = _context.Books.Include(b => b.Volumes).ThenInclude(v => v.Rents).Where(x => x.Id == bookId).AsNoTracking().FirstOrDefault();

            if (book == null)
                return NotFound();

            ViewBag.Title = book.Title + "Könyv információk: ";

            return View("Details", book);
        }

        //Rent specific book
        [HttpGet]
        public IActionResult Rent(Int32 volumeId)
        {
            var model = new RentViewModel()
            {
                VolumeId = volumeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rent(RentViewModel model)
        {
            if (TryValidateModel(model))
            {
                //Date validation
                if (model.StartDate < DateTime.Today || model.EndDate < DateTime.Today || model.StartDate > model.EndDate)
                {
                    model.ValidationMessage = "Érvénytelen dátumok.";

                    return View(model);
                }
                //Check if book can be rented on valid date
                if (_context.Rents.Any(r => r.EndDate >= model.StartDate && r.StartDate <= model.EndDate && r.VolumeId == model.VolumeId))
                {
                    model.ValidationMessage = "A kötet már le van foglalva a megjelölt időpontban.";

                    return View(model);
                }
                else
                {
                    //User can rent it, save order into database
                    var newRent = new Rent
                    {
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        VisitorId = 1,
                        VolumeId = model.VolumeId
                    };

                    _context.Rents.Add(newRent);

                    _context.SaveChanges();
                }
            }
            else
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
