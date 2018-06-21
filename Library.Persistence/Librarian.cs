using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Persistence
{
    public class Librarian
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Password { get; set; }
    }
}
