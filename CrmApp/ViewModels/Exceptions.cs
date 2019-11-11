using System;

namespace Crm.App.ViewModels
{
    public class OrderSavingException : Exception
    {
        public OrderSavingException() : base("Error saving an order.")
        {
        }

        public OrderSavingException(string message) : base(message)
        {
        }

        public OrderSavingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class OrderDeletionException : Exception
    {
        public OrderDeletionException() : base("Error deleting an order.")
        {
        }

        public OrderDeletionException(string message) : base(message)
        {
        }

        public OrderDeletionException(string message,
            Exception innerException) : base(message, innerException)
        {
        }
    }
}
