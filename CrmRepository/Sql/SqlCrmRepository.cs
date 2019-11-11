using Crm.Models;
using Microsoft.EntityFrameworkCore;

namespace Crm.Repository.Sql
{
    public class SqlCrmRepository : ICrmRepository
    {
        private readonly DbContextOptions<CrmContext> _dbOptions; 

        public SqlCrmRepository(DbContextOptionsBuilder<CrmContext> 
            dbOptionsBuilder)
        {
            _dbOptions = dbOptionsBuilder.Options;
            using (var db = new CrmContext(_dbOptions))
            {
                db.Database.EnsureCreated(); 
            }
        }

        public ICustomerRepository Customers => new SqlCustomerRepository(
            new CrmContext(_dbOptions));

        public IOrderRepository Orders => new SqlOrderRepository(
            new CrmContext(_dbOptions));

        public IProductRepository Products => new SqlProductRepository(
            new CrmContext(_dbOptions));
    }
}
