using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerTableTest.BLL;
using CustomerTableTest.Models;
using CustomerTableTestAPI.Models;



namespace CustomerTableTestAPI.Controllers
{
   
        [Authorize]
        [Route("api/[controller]")]
        [ApiController]
        public class CustomerController : ControllerBase
        {
            private readonly ICustomerService _customerService;

            public CustomerController(ICustomerService customerService)
            {
                _customerService = customerService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound();

                return Ok(customer);
            }

            [HttpPost]
            public async Task<IActionResult> Add([FromBody] Customer customer)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if Code is unique (business rule example)
                var existing = await _customerService.GetCustomerByCodeAsync(customer.Code);
                if (existing != null)
                    return BadRequest(new { message = "Code must be unique" });

                await _customerService.AddCustomerAsync(customer);
                return Ok(new { message = "Customer added successfully!" });
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
            {
                if (id != customer.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var existing = await _customerService.GetCustomerByIdAsync(id);
                if (existing == null)
                    return NotFound();

                await _customerService.UpdateCustomerAsync(customer);
                return Ok(new { message = "Customer updated successfully!" });
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound();

                await _customerService.DeleteCustomerAsync(customer);
                return Ok(new { message = "Customer deleted successfully!" });
            }
        }
    
    }

