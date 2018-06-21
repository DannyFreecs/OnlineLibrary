using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Persistence
{
    public class Rent
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int VolumeId { get; set; }
        public int VisitorId { get; set; }

        public Volume Volume { get; set; }
        public Visitor Visitor { get; set; }
    }
}
