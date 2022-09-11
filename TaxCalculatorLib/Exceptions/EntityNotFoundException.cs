using System;

namespace AndriiCo.TaxCalculatorLib.Exceptions
{
    /// <summary>
    /// An exception that is thrown when some entity is not found in a data source
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
