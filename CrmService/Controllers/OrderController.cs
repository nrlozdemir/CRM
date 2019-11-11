using Crm.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crm.Service.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _repository; 

        public OrderController(IOrderRepository repository)
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
            var order = await _repository.GetAsync(id); 
            if (order == null)
            {
                return NotFound(); 
            }
            return Ok(order); 
        }

        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerOrders(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(); 
            }
            var orders = await _repository.GetForCustomerAsync(id);
            if (orders == null)
            {
                return NotFound(); 
            }
            return Ok(orders); 
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return BadRequest(); 
            }
            var orders = await _repository.GetAsync(value); 
            if (orders == null)
            {
                return NotFound(); 
            }
            return Ok(orders); 
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Order order)
        {
            return Ok(await _repository.UpsertAsync(order)); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Order order)
        {
            await _repository.DeleteAsync(order.Id);
            return Ok(); 
        }
    }
}
