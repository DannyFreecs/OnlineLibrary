using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Persistence
{
    public class Book
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Author { get; set; }
        public int ReleaseYear { get; set; }
        public String ISBN { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<Volume> Volumes { get; set; }
    }
}
