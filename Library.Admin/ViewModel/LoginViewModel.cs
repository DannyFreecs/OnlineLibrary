using Library.Admin.Persistence;
using Library.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Library.Admin.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private ILibraryService _libraryService;

        // Kilépés parancsának lekérdezése.
        public DelegateCommand ExitCommand { get; private set; }

        // Bejelentkezés parancs lekérdezése.
        public DelegateCommand LoginCommand { get; private set; }

        // Felhasználónév lekérdezése, vagy beállítása.
        public String UserName { get; set; }

        // Alkalmazásból való kilépés eseménye.
        public event EventHandler ExitApplication;

        // Sikeres bejelentkezés eseménye.
        public event EventHandler LoginSuccess;

        // Sikertelen bejelentkezés eseménye.
        public event EventHandler LoginFailed;

        //Nézetmodell példányosítása
        public LoginViewModel(ILibraryService service)
        {
            _libraryService = service ?? throw new ArgumentNullException(nameof(service));

            UserName = String.Empty;

            ExitCommand = new DelegateCommand(param => OnExitApplication());

            LoginCommand = new DelegateCommand(param => LoginAsync(param as PasswordBox));
        }

        //Jelszótároló vezérlő
        private async void LoginAsync(PasswordBox passwordBox)
        {
            if (passwordBox == null)
                return;

            try
            {
                // a bejelentkezéshez szükségünk van a jelszótároló vezérlőre, mivel a jelszó tulajdonság nem köthető
                Boolean result = await _libraryService.LoginAsync(UserName, passwordBox.Password);

                if (result)
                    OnLoginSuccess();
                else
                    OnLoginFailed();
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Nincs kapcsolat a kiszolgálóval.");
            }
        }

        private void OnLoginSuccess()
        {
            LoginSuccess?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoginFailed()
        {
            LoginFailed?.Invoke(this, EventArgs.Empty);
        }


        private void OnExitApplication()
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

    }
}
