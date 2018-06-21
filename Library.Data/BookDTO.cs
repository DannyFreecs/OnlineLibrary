using System;
using System.Collections.Generic;

namespace Library.Data
{
    public class BookDTO
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Author { get; set; }
        public int ReleaseYear { get; set; }
        public String ISBN { get; set; }
        public byte[] Picture { get; set; }

        public IList<VolumeDTO> Volumes { get; set; }
    }
}
