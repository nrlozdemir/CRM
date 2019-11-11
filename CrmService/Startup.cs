using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Crm.Models;
using Crm.Repository;
using Crm.Repository.Sql;

namespace Crm.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var db = new CrmContext(new DbContextOptionsBuilder<CrmContext>()
                .UseSqlServer(Constants.SqlAzureConnectionString).Options);
            services.AddScoped<ICustomerRepository, SqlCustomerRepository>(_ => new SqlCustomerRepository(db));
            services.AddScoped<IOrderRepository, SqlOrderRepository>(_ => new SqlOrderRepository(db));
            services.AddScoped<IProductRepository, SqlProductRepository>(_ => new SqlProductRepository(db)); 
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
