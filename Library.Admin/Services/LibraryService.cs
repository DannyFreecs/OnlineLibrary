using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Data;
using Library.Admin.Persistence;
using System.Collections.ObjectModel;

namespace Library.Admin.Services
{
    public class LibraryService : ILibraryService
    {
        //Adat állapotjelzések
        private enum DataFlag
        {
            Create,
            Update,
            Delete
        }

        private ILibraryPersistence _persistence;
        private List<BookDTO> _books;
        private List<VolumeDTO> _volumes;
        private List<RentDTO> _rents;
        private Dictionary<BookDTO, DataFlag> _bookFlags;

        //Modell példányosítása
        public LibraryService(ILibraryPersistence persistence)
        {
            _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));

            IsUserLoggedIn = false;
        }

        // Bejelentkezettség lekérdezése.
        public Boolean IsUserLoggedIn { get; private set; }

        //Könyvek lekérdezése
        public IReadOnlyList<BookDTO> Books
        {
            get { return _books; }
        }

        //Kötetek lekérdezése
        public IReadOnlyList<VolumeDTO> Volumes
        {
            get { return _volumes; }
        }

        //Rendelések lekérdezése
        public IReadOnlyList<RentDTO> Rents
        {
            get { return _rents; }
        }

        //Könyv hozzáadása
        public void CreateBook(BookDTO book)
        {
            if(book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            if (_books.Contains(book))
            {
                throw new ArgumentException("The book is already in hte collection.", nameof(book));
            }

            book.Id = _books.Count > 0 ? _books.Max(b => b.Id) : 0; //generálunk új ideiglenes azonosítót, ami nem fog átkerülni a szerverre
            _bookFlags.Add(book, DataFlag.Create);
            _books.Add(book);
        }

        

        //Adatok betöltése
        public async Task LoadAsync()
        {
            //adatok
            _books = (await _persistence.ReadBooksAsync()).ToList();

            foreach (var book in _books)
            {
                // todo: ezt egyben kellene lekérni és nem egyessével
                var allVolume = await _persistence.ReadVolumesAsync(book.Id);
                book.Volumes = new ObservableCollection<VolumeDTO>(allVolume);
            }

            //állapotjelzések
            _bookFlags = new Dictionary<BookDTO, DataFlag>();
        }

        //Kötet adatinak betöltése
        public async Task LoadVolumesAsync(int bookID)
        {
            _volumes = (await _persistence.ReadVolumesAsync(bookID)).ToList();
        }

        //Adatok mentése
        public async Task SaveAsync()
        {
            //könyvek
            List<BookDTO> booksToSave = _bookFlags.Keys.ToList();

            foreach(BookDTO book in booksToSave)
            {
                Boolean result = true;

                //állapotjelzőnek megfelelő mávelet elvégzése
                switch(_bookFlags[book])
                {
                    case DataFlag.Create:
                        result = await _persistence.CreateBookAsync(book);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + _bookFlags[book] + " failed on book " + book.Id);

                //ha sikeres volt a ments => törölhetjük az állapotjelzőt
                _bookFlags.Remove(book);
            }
        }

        public async Task<int> AddVolume(BookDTO book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            if (!_books.Contains(book))
            {
                throw new ArgumentException("The book ID not found.", nameof(book));
            }

            var result = await _persistence.CreateVolumeAsync(book);

            return result;
        }

        public async Task RemoveVolume(VolumeDTO volume)
        {
            var volRents = Rents.Where(r => r.VolumeId == volume.Id).ToList();

            if(volRents.Any(x => x.IsActive))
            {
                throw new InvalidOperationException("Kiadott kötet törlése nem engedélyezett.");
            }

            await _persistence.RemoveVolumeAsync(volume);
        }

        public async Task LoadRentsAsync()
        {
            //adatok
            _rents = (await _persistence.ReadRentsAsync()).ToList();
        }

        //Rendelés módosítása
        public async Task UpdateRent(RentDTO rent)
        {
            if (rent == null)
                throw new ArgumentNullException(nameof(rent));

            if(VolumeAlreadyRented(rent))
            { 
                throw new InvalidOperationException("Ez a kötet jelenleg ki van adva!");
            }

            await _persistence.UpdateRentAsync(rent);
        }

        public bool VolumeAlreadyRented(RentDTO rent)
        {
            var volRents = Rents.Where(r => r.VolumeId == rent.VolumeId && r.IsActive).ToList();

            if(volRents.Count == 1 && volRents.First().Id != rent.Id)
            {
                return true;
            }

            return false;
        }

        public void CreateImage(BookDTO book, byte[] img)
        {
            if (book == null)
                throw new ArgumentException("The building does not exist.", nameof(book));

            // létrehozzuk a képet
            book.Picture = img;
        }

        //Bejelentkezés
        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            IsUserLoggedIn = await _persistence.LoginAsync(userName, userPassword);
            return IsUserLoggedIn;
        }

        //Kijelentkezés
        public async Task<Boolean> LogoutAsync()
        {
            if (!IsUserLoggedIn)
                return true;

            IsUserLoggedIn = !(await _persistence.LogoutAsync());

            return IsUserLoggedIn;
        }
    }
}
