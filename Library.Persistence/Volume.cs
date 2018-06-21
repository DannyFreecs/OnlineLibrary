using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Persistence
{
    public class Volume
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        public Book Book { get; set; }
        public ICollection<Rent> Rents { get; set; }
    }
}
