using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crm.Models
{    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAsync();

        Task<IEnumerable<Customer>> GetAsync(string search);

        Task<Customer> GetAsync(Guid id);

        Task<Customer> UpsertAsync(Customer customer);

        Task DeleteAsync(Guid customerId);
    }
}