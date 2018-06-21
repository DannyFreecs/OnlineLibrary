using Library.Data;
using System;

namespace Library.Admin.Services
{
    public class BookEventArgs : EventArgs
    {
        public BookDTO Book { get; set; }
    }
}
