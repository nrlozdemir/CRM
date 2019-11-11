using Crm.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crm.Repository.Rest
{
    public class RestProductRepository : IProductRepository
    {
        private readonly HttpHelper _http;

        public RestProductRepository(string baseUrl)
        {
            _http = new HttpHelper(baseUrl);
        }

        public async Task<IEnumerable<Product>> GetAsync() =>
            await _http.GetAsync<IEnumerable<Product>>("product"); 

        public async Task<Product> GetAsync(Guid id) => 
            await _http.GetAsync<Product>($"product/{id}");

        public async Task<IEnumerable<Product>> GetAsync(string search) =>
            await _http.GetAsync<IEnumerable<Product>>($"product/search?value={search}");
    }
}
