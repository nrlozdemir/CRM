using System;
using System.Collections.Generic;
using System.Linq;

namespace Crm.Models
{
    public class Order : DbObject
    {
        public Order()
        { }

        public Order(Customer customer) : this()
        {
            Customer = customer;
            CustomerName = $"{customer.FirstName} {customer.LastName}";
            Address = customer.Address;
        }

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public string CustomerName { get; set; }

        public int InvoiceNumber { get; set; } = 0;

        public string Address { get; set; }

        public List<LineItem> LineItems { get; set; } = new List<LineItem>();

        public DateTime DatePlaced { get; set; } = DateTime.Now;

        public DateTime? DateFilled { get; set; } = null;

        public OrderStatus Status { get; set; } = OrderStatus.Acik;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Odenmedi;

        public Term Term { get; set; }

        public decimal Subtotal => LineItems.Sum(lineItem => lineItem.Product.ListPrice * lineItem.Quantity);

        public decimal Tax => Subtotal * .095m;

        public decimal GrandTotal => Subtotal + Tax; 

        public override string ToString() => InvoiceNumber.ToString();
    }

    public enum Term
    {
        Vade, 
        Pesin,
        Senet, 
        AySonu
    }

    public enum PaymentStatus
    {
        Odenmedi,
        Odendi 
    }
    public enum OrderStatus
    {
        Acik,
        Islemde, 
        Iptal
    }
}
