using Crm.Models;
using Microsoft.EntityFrameworkCore;

namespace Crm.Repository.Sql
{
    public class CrmContext : DbContext
    {
        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        { }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<LineItem> LineItems { get; set; }
    }
}
