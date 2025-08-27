//using AutoMapper;
using CustomerTableTest.BLL.Services;
using CustomerTableTest.Common.Responses;
using CustomerTableTest.Models;
using CustomerTableTest.Models.DTOs; // مهم
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CustomerTableTestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<BaseResponse<CustomerDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return BaseResponse<CustomerDto>.Ok(dto);
    }

    [HttpGet]
    public async Task<BaseResponse<List<CustomerDto>>> List()
    {
        var list = await _service.ListAsync();
        return BaseResponse<List<CustomerDto>>.Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // حسب طلباتك لتقييد الإضافة
    public async Task<BaseResponse<CustomerDto>> Create([FromBody] CustomerDto dto)
    {
     

        var created = await _service.CreateAsync(dto);

        return BaseResponse<CustomerDto>.Ok(created, "Customer created");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<BaseResponse<CustomerDto>> Update(int id, [FromBody] CustomerDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return BaseResponse<CustomerDto>.Ok(updated, "Customer updated");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<BaseResponse<object>> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return BaseResponse<object>.Ok(null, "Customer deleted");
    }
}



//using CustomerTableTest.BLL;
//using CustomerTableTest.Models;
//using CustomerTableTestAPI.Models.DTOs;
//using CustomerTableTestAPI.Models.Responses;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CustomerTableTestAPI.Controllers
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CustomerController : ControllerBase
//    {
//        private readonly ICustomerService _customerService;
//        private readonly IMapper _mapper;

//        public CustomerController(ICustomerService customerService, IMapper mapper)
//        {
//            _customerService = customerService;
//            _mapper = mapper;
//        }

//        [HttpGet]
//        public async Task<BaseResponse<List<CustomerDto>>> GetAll()
//        {
//            var customers = await _customerService.GetAllCustomersAsync();
//            var CustomerDto = _mapper.Map<List<CustomerDto>>(customers);
//            return new BaseResponse<List<CustomerDto>>(data: CustomerDto);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var customer = await _customerService.GetCustomerByIdAsync(id);
//            if (customer == null)
//                return NotFound(new BaseResponse<string>(message: "Customer not found", success: false));

//            var customerDto = _mapper.Map<CustomerDto>(customer);
//            return Ok(new BaseResponse<CustomerDto>(data: customerDto));
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Add([FromBody] CustomerDto customerDto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new BaseResponse<string>(message: "Invalid data", success: false));

//            var customer = _mapper.Map<Customer>(customerDto);
//            var result = await _customerService.AddCustomerAsync(customer);

//            if (!result.IsSuccess)
//                return BadRequest(new BaseResponse<string>(message: result.Message, success: false));

//            return Ok(new BaseResponse<string>(message: "Customer added successfully!", success: true));
//        }

//        [HttpPut("{id}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Update(int id, [FromBody] CustomerDto customerDto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(new BaseResponse<string>(message: "Invalid data", success: false));

//            var customer = _mapper.Map<Customer>(customerDto);
//            customer.Id = id;

//            var result = await _customerService.UpdateCustomerAsync(id, customer);

//            if (!result.IsSuccess)
//                return BadRequest(new BaseResponse<string>(message: result.Message, success: false));

//            return Ok(new BaseResponse<string>(message: "Customer updated successfully!", success: true));
//        }

//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var result = await _customerService.DeleteCustomerAsync(id);

//            if (!result.IsSuccess)
//                return BadRequest(new BaseResponse<string>(message: result.Message, success: false));

//            return Ok(new BaseResponse<string>(message: "Customer deleted successfully!", success: true));
//        }
//    }
//}
