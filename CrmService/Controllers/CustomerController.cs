using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Crm.Models;

namespace Crm.Service.Controllers
{
    namespace Crm.Service.Controllers
    {
        [Route("api/[controller]")]
        public class CustomerController : Controller
        {
            private ICustomerRepository _repository; 

            public CustomerController(ICustomerRepository repository)
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
                var customer = await _repository.GetAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(customer); 
                }
            }

            [HttpGet("search")]
            public async Task<IActionResult> Search(string value)
            {
                return Ok(await _repository.GetAsync(value)); 
            }

            [HttpPost]
            public async Task<IActionResult> Post([FromBody]Customer customer)
            {
                return Ok(await _repository.UpsertAsync(customer)); 
            }

            /// <summary>
            /// Deletes a customer and all data associated with them.
            /// </summary>
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                await _repository.DeleteAsync(id);
                return Ok(); 
            }
        }
    }
}
