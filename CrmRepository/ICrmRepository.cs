using Crm.Models;

namespace Crm.Repository
{
    public interface ICrmRepository
    {
        ICustomerRepository Customers { get; }

        IOrderRepository Orders { get; }

        IProductRepository Products { get;  }
    }
}
