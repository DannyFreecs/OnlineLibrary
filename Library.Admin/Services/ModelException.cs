using System;

namespace Library.Admin.Services
{
    public class ModelException : Exception
    {
        public ModelException() { }

        public ModelException(Exception innerException) : base(String.Empty, innerException) { }
    }
}
