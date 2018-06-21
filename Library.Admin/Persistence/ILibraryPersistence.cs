using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Data;

namespace Library.Admin.Persistence
{
    //perzisztencia felülete
    public interface ILibraryPersistence
    {
        //Könyvek olvasása
        Task<IEnumerable<BookDTO>> ReadBooksAsync();

        //könyv létrehozása
        Task<Boolean> CreateBookAsync(BookDTO book);

        //Kötetek olvasása
        Task<IEnumerable<VolumeDTO>> ReadVolumesAsync(int bookID);

        //Kötet hozzádása adott könyvhöz
        Task<int>CreateVolumeAsync(BookDTO book);

        //Kötet törlése
        Task RemoveVolumeAsync(VolumeDTO volume);

        //Jövőbeni/Aktív rendelések
        Task<IEnumerable<RentDTO>> ReadRentsAsync();

        //Kölcsönzés módosítása
        Task UpdateRentAsync(RentDTO rent);

        //Bejelentkezés.
        Task<Boolean> LoginAsync(String userName, String userPassword);

        // Kijelentkezés.
        Task<Boolean> LogoutAsync();
    }
}
