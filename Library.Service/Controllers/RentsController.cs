using System;
using System.Linq;
using Library.Data;
using Library.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Rents")]
    public class RentsController : Controller
    {
        private LibraryContext _context;

        //Vezérlő példányosítása
        public RentsController(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //Rendelések lekérdezése
        // GET api/values
        [HttpGet]
        public IActionResult GetRents()
        {
            try
            {
                return Ok(_context.Rents.ToList().Select(rent => new RentDTO
                {
                    Id = rent.Id,
                    VolumeId = rent.VolumeId,
                    VisitorId = rent.VisitorId,
                    StartDate = rent.StartDate,
                    EndDate = rent.EndDate,
                    IsActive = rent.IsActive
                }).Where(x => x.IsActive || x.StartDate >= DateTime.Today));
            }
            catch
            {
                //Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        // PUT: api/Rents/5
        [HttpPut]
        public IActionResult PutRent([FromBody] RentDTO rentDTO)
        {
            try
            {
                Rent rent = _context.Rents.FirstOrDefault(r => r.Id == rentDTO.Id);

                if (rent == null) //ha nincs ilyen azonosítójú => hiba
                    return NotFound();

                rent.IsActive = !rentDTO.IsActive;

                _context.SaveChanges(); //elmentjük a módosított épületet

                return Ok();
            }
            catch
            {
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
