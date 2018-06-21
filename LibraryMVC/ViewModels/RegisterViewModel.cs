using System.ComponentModel.DataAnnotations;

namespace Bead1.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(60, ErrorMessage = "A foglaló neve maximum 60 karakter lehet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A lakcím megadása kötelező.")]
        [RegularExpression(@"\d{4}\s+\D{1,20},?\s+\D{1,30}\s+\D{1,10}\s+\d+\.?", ErrorMessage="pl: 2200 Monor, Petőfi Sándor utca 43.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "A telefonszám megadása kötelező.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Jelszó megadása kötelező.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "A két jelszó nem egyezik!")]
        public string PasswordAgain { get; set; }
    }
}
