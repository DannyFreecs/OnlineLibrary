using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Data
{
    public class VolumeDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }

       public IList<RentDTO> Rents { get; set; }
    }
}
