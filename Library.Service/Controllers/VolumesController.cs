using System;
using System.Linq;
using Library.Data;
using Library.Persistence;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Volumes")]
    public class VolumesController : Controller
    {
        private LibraryContext _context;

        public VolumesController(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        // POST: api/Volumes
        [HttpPost]
        public IActionResult Post([FromBody] BookDTO bookDto)
        {
            try
            {
                var addedBook = _context.Volumes.Add(new Volume
                {
                    BookId = bookDto.Id
                });

                _context.SaveChanges(); //új könyv elmentése

                bookDto.Id = addedBook.Entity.Id;

                //visszaküldjük a létrehozott épületet
                return Created(Request.GetUri() + addedBook.Entity.Id.ToString(), bookDto);
            }
            catch
            {
                // Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Volume volume = _context.Volumes.FirstOrDefault(b => b.Id == id);
                var rents = _context.Rents.Where(r => r.VolumeId == id).ToList();

                if (volume == null)
                {
                    return NotFound();
                }

                _context.Volumes.Remove(volume);

                for (int i = 0; i < rents.Count; i++)
                {
                    _context.Rents.Remove(rents.ElementAt(i));
                }

                _context.SaveChanges();

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
