using System;
using System.Windows;
using Library.Admin.Persistence;
using Library.Admin.Services;
using Library.Admin.View;
using Library.Admin.ViewModel;
using Library.Data;
using Microsoft.Win32;

namespace Library.Admin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILibraryService _service;
        private LoginViewModel _loginViewModel;
        private LoginWindow _loginView;
        private MainViewModel _viewModel;
        private MainWindow _view;
        private BookCreatingWindow _editorView;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _service = new LibraryService(new LibraryServicePersistence("http://localhost:52954/")); // megadjuk a szolgáltatás címét

            _loginViewModel = new LoginViewModel(_service);
            _loginViewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _loginViewModel.LoginSuccess += new EventHandler(ViewModel_LoginSuccess);
            _loginViewModel.LoginFailed += new EventHandler(ViewModel_LoginFailed);

            _loginView = new LoginWindow();
            _loginView.DataContext = _loginViewModel;
            _loginView.Show();

            
        }

        private void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("A bejelentkezés sikertelen!", "Utazási ügynökség", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_LoginSuccess(object sender, EventArgs e)
        {
            _viewModel = new MainViewModel(_service);
            _viewModel.MessageApplication += new EventHandler<MessageEventArgs>(ViewModel_MessageApplication);
            _viewModel.ExitApplication += new EventHandler(ViewModel_ExitApplication);
            _viewModel.BookCreatingStarted += new EventHandler(MainViewModel_BookCreatingStarted);
            _viewModel.BookCreatingFinished += new EventHandler(MainViewModel_BookCreatingFinished);
            _viewModel.ImageEditingStarted += new EventHandler<BookEventArgs>(MainViewModel_ImageEditingStarted);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();

            _loginView.Close();
        }

        private void MainViewModel_ImageEditingStarted(object sender, BookEventArgs e)
        {
            try
            {
                // egy dialógusablakban bekérjük a fájlnevet
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.CheckFileExists = true;
                dialog.Filter = "Képfájlok|*.jpg;*.jpeg;*.bmp;*.tif;*.gif;*.png;";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                Boolean? result = dialog.ShowDialog();

                if (result == true)
                {
                    // kép létrehozása (a megfelelő méretekkel)
                    _service.CreateImage(e.Book, ImageHandler.OpenAndResize(dialog.FileName, 600));
                }
            }
            catch { }
        }

        private void MainViewModel_BookCreatingStarted(object sender, EventArgs e)
        {
            _editorView = new BookCreatingWindow(); // külön szerkesztő dialógus az épületekre
            _editorView.DataContext = _viewModel;
            _editorView.Show();
        }

        private void MainViewModel_BookCreatingFinished(object sender, EventArgs e)
        {
            _editorView.Close();
        }

        private void ViewModel_MessageApplication(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Könyvtár", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_ExitApplication(object sender, EventArgs e)
        {
            Shutdown();
        }
    }
}
