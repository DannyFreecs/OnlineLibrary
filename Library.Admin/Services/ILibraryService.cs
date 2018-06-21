using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Data;

namespace Library.Admin.Services
{
    //Könyvtár modell felülete
    public interface ILibraryService
    {
        //Könyvek lekérdezése
        IReadOnlyList<BookDTO> Books { get; }

        //Kötetek lekérdezése
        IReadOnlyList<VolumeDTO> Volumes { get; }

        //Rendelések lekérdezése
        IReadOnlyList<RentDTO> Rents { get; }

        //Könyv hozzáadása
        void CreateBook(BookDTO book);

        //Könyvek betöltése
        Task LoadAsync();

        //Adatok mentése
        Task SaveAsync();

        //Kötetek betöltése
        Task LoadVolumesAsync(int bookID);

        //Kötet hozzáadása könyvhöz
        Task<int> AddVolume(BookDTO book);

        //Kötet törlése
        Task RemoveVolume(VolumeDTO volume);

        //Aktív/Jövőbeni rendelések betöltése
        Task LoadRentsAsync();

        Task UpdateRent(RentDTO rent);

        void CreateImage(BookDTO book, Byte[] img);

        // Bejelentkezés.
        Task<Boolean> LoginAsync(String userName, String userPassword);

        // Kijelentkezés.
        Task<Boolean> LogoutAsync();
    }
}
