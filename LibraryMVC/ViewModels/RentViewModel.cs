using System;
using System.ComponentModel.DataAnnotations;

namespace Bead1.ViewModels
{
    public class RentViewModel : ViewModelBase
    {
        public int VolumeId { get; set; }

        [Required(ErrorMessage ="Kölcsönzés kezdőnapja kötelező!")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Kölcsönzés zárónapja kötelező!")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
    }
}
