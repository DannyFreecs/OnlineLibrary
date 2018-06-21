using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Data
{
    public class RentDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int VolumeId { get; set; }
        public int VisitorId { get; set; }

       // public Volume Volume { get; set; }
       // public Visitor Visitor { get; set; }
    }
}
