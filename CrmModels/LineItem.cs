using Newtonsoft.Json;
using System;

namespace Crm.Models
{
    public class LineItem : DbObject
    {
        public Guid OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; } = 1; 
    }
}