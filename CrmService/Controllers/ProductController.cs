using Crm.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crm.Service.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _repository.GetAsync()); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(); 
            }
            var products = await _repository.GetAsync(id); 
            if (products == null)
            {
                return NotFound(); 
            }
            return Ok(products); 
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return BadRequest();
            }
            var products = await _repository.GetAsync(value);
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }
    }
}
