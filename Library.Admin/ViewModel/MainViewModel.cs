using System;
using System.Collections.ObjectModel;
using Library.Admin.Persistence;
using Library.Admin.Services;
using Library.Data;

namespace Library.Admin.ViewModel
{
    //Nézetmodell típusa
    public class MainViewModel : ViewModelBase
    {
        private ILibraryService _libraryService;
        private ObservableCollection<BookDTO> _books;
        private BookDTO _selectedBook;
        private VolumeDTO _selectedVolume;
        private BookDTO _currentBook;
        private Boolean _isLoaded;
        private Int32 _selectedIndex;
        private ObservableCollection<RentDTO> _rents;
        private RentDTO _selectedRent;



        public ObservableCollection<RentDTO> Rents
        {
            get => _rents;
            private set
            {
                if (_rents != value)
                {
                    _rents = value;
                    OnPropertyChanged();
                }
            }
        }

        public RentDTO SelectedRent
        {
            get => _selectedRent;
            set
            {
                if (SelectedRent != value)
                {
                    _selectedRent = value;
                    OnPropertyChanged();
                }
            }
        }

        //Könyvek lekérdezése
        public ObservableCollection<BookDTO> Books
        {
            get { return _books; }
            private set
            {
                if (_books != value)
                {
                    _books = value;
                    OnPropertyChanged();
                }
            }
        }

        public BookDTO SelectedBook
        {
            get => _selectedBook;
            set
            {
                if(_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged();
                }
            }
        }

        public VolumeDTO SelectedVolume
        {
            get => _selectedVolume;
            set
            {
                if(SelectedVolume != value)
                {
                    _selectedVolume = value;
                    OnPropertyChanged();
                }
            }
        }

        //Új könyv lekérdezése
        public BookDTO CurrentBook
        {
            get { return _currentBook; }
            private set
            {
                if (_currentBook != value)
                {
                    _currentBook = value;
                    OnPropertyChanged();
                }
            }
        }

        //Betöltöttség lekérdezése
        public Boolean IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        // A kiválasztott elem indexének lekérdezése, vagy beállítása.
        public Int32 SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();

                    // létrehozunk egy új aktuális könyvet, amibe bemásoljuk a kijelölt könyv adatait
                    if (_selectedIndex >= 0 && _selectedIndex < _books.Count)
                        CurrentBook = new BookDTO
                        {
                            Id = _books[_selectedIndex].Id,
                            Title = _books[_selectedIndex].Title,
                            Author = _books[_selectedIndex].Author,
                            ReleaseYear = _books[_selectedIndex].ReleaseYear,
                            ISBN = _books[_selectedIndex].ISBN,
                            Picture = _books[_selectedIndex].Picture
                        };
                }
            }
        }

        public BookDTO NewBook { get; private set; }

        // Hozzáadás parancsának lekérdezése.
        public DelegateCommand CreateBookCommand { get; private set; }

        public DelegateCommand AddVolumeCommand { get; private set; }

        public DelegateCommand RemoveVolumeCommand { get; private set; }

        // Kilépés parancsának lekérdezése.
        public DelegateCommand ExitCommand { get; private set; }

        // Betöltés parancsának lekérdezése.
        public DelegateCommand LoadCommand { get; private set; }


        // Mentés parancsának lekérdezése.
        public DelegateCommand SaveCommand { get; private set; }

        //Kötet lekérés parancsának lekérdezése.
        public DelegateCommand LoadVolumesCommand { get; private set; }

        //Rendelések betöltés parancs lekérdezése
        public DelegateCommand LoadRentsCommand { get; private set; }

        //Rendelés módosítása
        public DelegateCommand UpdateRentCommand { get; private set; }

        public DelegateCommand SaveChangesCommand { get; private set; }

        public DelegateCommand CancelChangesCommand { get; private set; }

        public DelegateCommand CreateImageCommand { get; private set; }

        // Alkalmazásból való kilépés eseménye.
        public event EventHandler ExitApplication;

        public event EventHandler BookCreatingStarted;

        public event EventHandler BookCreatingFinished;

        public event EventHandler<BookEventArgs> ImageEditingStarted;

        // Nézetmodell példányosítása.
        public MainViewModel(ILibraryService model)
        {
            _libraryService = model;
            _isLoaded = false;

            CreateBookCommand = new DelegateCommand(param =>
            {
                NewBook = new BookDTO();
                OnBookCreatingStarted();
            });

            AddVolumeCommand = new DelegateCommand(async param => {

                if (SelectedBook == null)
                {
                    return;
                }

                IsBusy = true;

                try
                {
                    var result = await _libraryService.AddVolume(SelectedBook);
                    SelectedBook.Volumes.Add(new VolumeDTO { BookId = SelectedBook.Id, Id = result });
                }
                catch (Exception e)
                {
                    OnMessageApplication(e.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            });

            RemoveVolumeCommand = new DelegateCommand(async param => {
                if (SelectedVolume == null)
                {
                    return;
                }

                IsBusy = true;
                try
                {
                    await _libraryService.RemoveVolume(SelectedVolume);
                    SelectedBook.Volumes.Remove(SelectedVolume);
                }
                catch (Exception e)
                {
                    OnMessageApplication(e.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            });

            UpdateRentCommand = new DelegateCommand(async param =>
            {
                if (SelectedRent == null)
                {
                    return;
                }

                IsBusy = true;

                try
                {

                    await _libraryService.UpdateRent(SelectedRent);
                }
                catch (Exception e)
                {
                    OnMessageApplication(e.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            });

            CreateImageCommand = new DelegateCommand(param => OnImageEditingStarted(NewBook));

            SaveChangesCommand = new DelegateCommand(param => SaveChanges());
            CancelChangesCommand = new DelegateCommand(param => CancelChanges());
            LoadCommand = new DelegateCommand(param => LoadAsync());
            SaveCommand = new DelegateCommand(param => SaveAsync());
            LoadRentsCommand = new DelegateCommand(param => LoadRentsAsync());

            ExitCommand = new DelegateCommand(param => OnExitApplication());
        }

        private void OnImageEditingStarted(BookDTO b)
        {
            ImageEditingStarted?.Invoke(this, new BookEventArgs { Book = b });
        }

        private void CancelChanges()
        {
            NewBook = null;
            OnBookCreatingFinished();
        }

        private void SaveChanges()
        {
            // ellenőrzések
            if (String.IsNullOrEmpty(NewBook.Title))
            {
                OnMessageApplication("A cím nincs megadva!");
                return;
            }
            if (String.IsNullOrEmpty(NewBook.Author))
            {
                OnMessageApplication("A szerző nincs megadva!");
                return;
            }
            if (NewBook.ReleaseYear == 0)
            {
                OnMessageApplication("A kiadás éve nincs megadva");
                return;
            }
            if (String.IsNullOrEmpty(NewBook.ISBN))
            {
                OnMessageApplication("Az ISBN nincs megadva!");
                return;
            }
            if(NewBook.Picture == null)
            {
                OnMessageApplication("Könyvborító hiányzik!");
                return;
            }

            // mentés
            _libraryService.CreateBook(NewBook);
            Books.Add(NewBook);
            SelectedBook = NewBook;

            NewBook = null;

            OnBookCreatingFinished();
        }

        private void OnBookCreatingStarted()
        {
            BookCreatingStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnBookCreatingFinished()
        {
            BookCreatingFinished?.Invoke(this, EventArgs.Empty);
        }

        private async void LoadRentsAsync()
        {
            IsBusy = true;

            try
            {
                await _libraryService.LoadRentsAsync();
                Rents = new ObservableCollection<RentDTO>(_libraryService.Rents); // az adatokat egy követett gyűjteménybe helyezzük
                if(Books == null)
                {
                    LoadAsync();
                }
                IsLoaded = true;
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A betöltés sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Betöltés végrehajtása.
        /// </summary>
        private async void LoadAsync()
        {
            IsBusy = true;
            try
            {
                await _libraryService.LoadAsync();
                Books = new ObservableCollection<BookDTO>(_libraryService.Books); // az adatokat egy követett gyűjteménybe helyezzük
                if (Rents == null)
                {
                    LoadRentsAsync();
                }
                IsLoaded = true;
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A betöltés sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Mentés végreahajtása.
        private async void SaveAsync()
        {
            try
            {
                await _libraryService.SaveAsync();
                OnMessageApplication("A mentés sikeres!");
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("A mentés sikertelen! Nincs kapcsolat a kiszolgálóval.");
            }
        }

        // Alkalmazásból való kilépés eseménykiváltása.
        private void OnExitApplication()
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }
    }
}
