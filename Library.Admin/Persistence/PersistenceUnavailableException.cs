using System;

namespace Library.Admin.Persistence
{
    public class PersistenceUnavailableException : Exception
    {
        public PersistenceUnavailableException(String message) : base(message) { }

        public PersistenceUnavailableException(Exception innerException) : base("Exception occured.", innerException) { }
    }
}
