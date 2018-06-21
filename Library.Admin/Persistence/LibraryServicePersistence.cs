using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Library.Data;

namespace Library.Admin.Persistence
{
    //Szolgáltatás alapú perzisztencia

    public class LibraryServicePersistence : ILibraryPersistence
    {
        private HttpClient _client;

        //Szolgáltatás alapú perzisztencia példányosítása.
        public LibraryServicePersistence(string baseAddress)
        {
            _client = new HttpClient(); //szolgáltatás kliense
            _client.BaseAddress = new Uri(baseAddress); //megadjuk neki a címet
        }

        //Könyvek betöltése
        public async Task<IEnumerable<BookDTO>> ReadBooksAsync()
        {
            try
            {
                //kéréseket kliense keresztül végezzük
                HttpResponseMessage response = await _client.GetAsync("api/books/");
                if(response.IsSuccessStatusCode) //ha siekres a művelet
                {
                    IEnumerable<BookDTO> books = await response.Content.ReadAsAsync<IEnumerable<BookDTO>>();
                    //a tartalmat JSON formátumból objektumokká alakítjuk

                    return books;
                }
                else
                {
                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        //Kötetek betöltése adott könyvhöz
        public async Task<IEnumerable<VolumeDTO>> ReadVolumesAsync(int bookID)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/books/" + bookID.ToString());
                if (response.IsSuccessStatusCode) //ha siekres a művelet
                {
                    IEnumerable<VolumeDTO> volumes = await response.Content.ReadAsAsync<IEnumerable<VolumeDTO>>();
                    //a tartalmat JSON formátumból objektumokká alakítjuk

                    return volumes;
                }
                else
                {
                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        //Könyv létrehozása
        public async Task<bool> CreateBookAsync(BookDTO book)
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/books/", book); // az értékeket azonnal JSON formátumra alakítjuk
                book.Id = (await response.Content.ReadAsAsync<BookDTO>()).Id; // a válaszüzenetben megkapjuk a végleges azonosítót
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<int> CreateVolumeAsync(BookDTO book)
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/volumes/", book); // az értékeket azonnal JSON formátumra alakítjuk
                var id = (await response.Content.ReadAsAsync<VolumeDTO>()).Id; // a válaszüzenetben megkapjuk a végleges azonosítót

                return id;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task RemoveVolumeAsync(VolumeDTO volume)
        {
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync($"api/volumes/{volume.Id}");
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<IEnumerable<RentDTO>> ReadRentsAsync()
        {
            try
            {
                //kéréseket kliense keresztül végezzük
                HttpResponseMessage response = await _client.GetAsync("api/rents/");
                if (response.IsSuccessStatusCode) //ha siekres a művelet
                {
                    IEnumerable<RentDTO> rents = await response.Content.ReadAsAsync<IEnumerable<RentDTO>>();
                    //a tartalmat JSON formátumból objektumokká alakítjuk

                    return rents;
                }
                else
                {
                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task UpdateRentAsync(RentDTO rent)
        {
            try
            {
                HttpResponseMessage response = await _client.PutAsJsonAsync("api/rents/", rent);
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        // Bejelentkezés.
        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/account/login/" + userName + "/" + userPassword);
                return response.IsSuccessStatusCode; // a művelet eredménye megadja a bejelentkezés sikeressségét
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        // Kijelentkezés.
        public async Task<Boolean> LogoutAsync()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/account/logout");
                return !response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }
    }
}
