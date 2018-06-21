using Library.Data;
using Library.Persistence;
using Library.Service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Library.Service.Test
{
    public class LibraryTest : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly List<BookDTO> _bookDTOs;
        private readonly List<VolumeDTO> _volumeDTOs;
        private readonly List<RentDTO> _rentDTOs;

        public LibraryTest()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase("LibraryDataTest")
                .Options;

            _context = new LibraryContext(options);
            _context.Database.EnsureCreated();

            // adatok inicializációja
            var pics = new byte[] { 1, 2, 3, 4, 5 };
            var bookData = new List<Book>
            {
                new Book {Title="HP1", Author="JKR", ReleaseYear=1996, ISBN="123456", Picture = pics},
                new Book {Title="HP2", Author="JKR", ReleaseYear=1998, ISBN="234567", Picture = pics},
                new Book {Title="HP3", Author="JKR", ReleaseYear=2000, ISBN="345678", Picture = pics}
            };

            _context.Books.AddRange(bookData);


            var volumeData = new List<Volume>
            {
                new Volume {BookId = bookData[0].Id},
                new Volume {BookId = bookData[0].Id},
                new Volume {BookId = bookData[1].Id},
                new Volume {BookId = bookData[1].Id},
                new Volume {BookId = bookData[1].Id},
                new Volume {BookId = bookData[2].Id}
            };

            _context.Volumes.AddRange(volumeData);

            var rentData = new List<Rent>
            {
                new Rent {VolumeId = volumeData[0].Id, IsActive=true},
                new Rent {VolumeId = volumeData[0].Id, IsActive=false},
                new Rent {VolumeId = volumeData[0].Id, IsActive=false},
                new Rent {VolumeId = volumeData[1].Id, IsActive=false},
                new Rent {VolumeId = volumeData[1].Id, IsActive=true},
                new Rent {VolumeId = volumeData[2].Id, IsActive=false},
                new Rent {VolumeId = volumeData[2].Id, IsActive=false},
                new Rent {VolumeId = volumeData[2].Id, IsActive=true}
            };

            _context.Rents.AddRange(rentData);
            _context.SaveChanges();

            _bookDTOs = bookData.Select(book => new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ReleaseYear = book.ReleaseYear,
                ISBN = book.ISBN,
                Picture = book.Picture
            }).ToList();

            _volumeDTOs = volumeData.Select(vol => new VolumeDTO
            {
                Id = vol.Id,
                BookId = vol.BookId
            }).ToList();

            _rentDTOs = rentData.Select(rent => new RentDTO
            {
                Id = rent.Id,
                VolumeId = rent.VolumeId,
                IsActive = rent.IsActive
            }).ToList();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetBooksTest()
        {
            var controller = new BooksController(_context);
            var result = controller.GetBooks();

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BookDTO>>(objectResult.Value);
            var books = model.ToList();

            for(int i=0; i<_bookDTOs.Count; i++)
            {
                Assert.Equal(_bookDTOs[i].Id,          books[i].Id);
                Assert.Equal(_bookDTOs[i].Title,       books[i].Title);
                Assert.Equal(_bookDTOs[i].Author,      books[i].Author);
                Assert.Equal(_bookDTOs[i].ReleaseYear, books[i].ReleaseYear);
                Assert.Equal(_bookDTOs[i].ISBN,        books[i].ISBN);
                Assert.Equal(_bookDTOs[i].Picture,     books[i].Picture);
                Assert.Equal(_bookDTOs[i].Volumes,     books[i].Volumes);
            }         
        }

        [Fact]
        public void PostBookTest()
        {
            var pic = new byte[] { 1, 2, 3, 4, 5 };
            var newBook = new BookDTO
            {
                Title = "Receptek",
                Author = "Kandisz Nóra",
                ReleaseYear = 2005,
                ISBN = "55555",
                Picture = pic
            };

            var controller = new BooksController(_context);
            var result = controller.PostBook(newBook);

            // Assert

            Assert.Equal(_bookDTOs.Count + 1, _context.Books.Count());

            var created = _context.Books.Last();

            Assert.Equal(newBook.Id,          created.Id);
            Assert.Equal(newBook.Title,       created.Title);
            Assert.Equal(newBook.Author,      created.Author);
            Assert.Equal(newBook.ReleaseYear, created.ReleaseYear);
            Assert.Equal(newBook.ISBN,        created.ISBN);
            Assert.Equal(newBook.Picture,     created.Picture);
        }

        [Fact]
        public void GetBookTest()
        {
            var controller = new BooksController(_context);

            var result = controller.GetBook(_bookDTOs[0].Id);

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<VolumeDTO>>(objectResult.Value);
            var vols = model.ToList();

            for (int i = 0; i < vols.Count; i++)
            {
                Assert.Equal(_volumeDTOs[i].Id,     vols[i].Id);
                Assert.Equal(_volumeDTOs[i].BookId, vols[i].BookId);
            }
        }

        [Fact]
        public void PostVolumeTest()
        {
            var controller = new VolumesController(_context);
            var result = controller.Post(_bookDTOs[0]);
            var created = _context.Volumes.Last();

            // Assert
            Assert.Equal(_volumeDTOs.Count + 1, _context.Volumes.Count());
            Assert.Equal(_bookDTOs[0].Id, created.Id);
        }

        [Fact]
        public void RemoveVolumeTest()
        {
            var controller = new VolumesController(_context);
            var result = controller.Delete(_volumeDTOs[0].Id);
            var rents = _context.Rents.ToList();

            //Assert
            Assert.Equal(_volumeDTOs.Count - 1, _context.Volumes.Count());
            Assert.Equal(_rentDTOs.Count - 3, _context.Rents.Count());

            for(int i=0; i<rents.Count; i++)
            {
                Assert.False(rents[i].VolumeId == _volumeDTOs[0].Id);
            }
        }

        [Fact]
        public void GetRentsTest()
        {
            var controller = new RentsController(_context);
            var result = controller.GetRents();

            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<RentDTO>>(objectResult.Value);
            var rents = model.ToList();

            Assert.Equal(3, rents.Count);

            for(int i=0; i<3; i++)
            {
                Assert.True(rents[i].IsActive);
            }
        }

        [Fact]
        public void PutRent()
        {
            var controller = new RentsController(_context);
            controller.PutRent(_rentDTOs[0]);
            controller.PutRent(_rentDTOs[3]);

            var result1 = _context.Rents.FirstOrDefault(r => r.Id == _rentDTOs[0].Id);
            var result2 = _context.Rents.FirstOrDefault(r => r.Id == _rentDTOs[3].Id);

            //Assert
            Assert.Equal(_rentDTOs[0].IsActive, !result1.IsActive);
            Assert.Equal(_rentDTOs[3].IsActive, !result2.IsActive);
            Assert.Equal(_rentDTOs.Count, _context.Rents.Count());

        }
    }
}
