using System.ComponentModel.DataAnnotations;

namespace Bead1.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        [Required(ErrorMessage ="Felhasználó név megadása kötelező.")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Jelszó kitöltése kötelező.")]
        public string Password { get; set; }
    }
}
