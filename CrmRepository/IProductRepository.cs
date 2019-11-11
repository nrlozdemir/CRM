using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crm.Models
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAsync();

        Task<Product> GetAsync(Guid id);

        Task<IEnumerable<Product>> GetAsync(string search);
    }
}
