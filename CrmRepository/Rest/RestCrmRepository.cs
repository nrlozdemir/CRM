using Crm.Models;

namespace Crm.Repository.Rest
{
    public class RestCrmRepository : ICrmRepository
    {
        private readonly string _url; 

        public RestCrmRepository(string url)
        {
            _url = url; 
        }

        public ICustomerRepository Customers => new RestCustomerRepository(_url); 

        public IOrderRepository Orders => new RestOrderRepository(_url);

        public IProductRepository Products => new RestProductRepository(_url); 
    }
}
