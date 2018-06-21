using System.Collections.Generic;

namespace Library.Persistence
{
    public class Visitor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] Password { get; set; }

        public ICollection<Rent> Rents { get; set; }
    }
}
