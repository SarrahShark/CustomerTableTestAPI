using AutoMapper;
using CustomerTableTest.Common.Exceptions;
using CustomerTableTest.DAL.Repositories;
using CustomerTableTest.Models;
using CustomerTableTest.Models.DTOs; // مهم
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CustomerTableTest.BLL.Services;

public interface ICustomerService
{
    Task<CustomerDto> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CustomerDto dto);
    Task<CustomerDto> UpdateAsync(int id, CustomerDto dto);
    Task DeleteAsync(int id);
    Task<List<CustomerDto>> ListAsync();
}

public class CustomerService : ICustomerService
{
    private readonly IBaseRepository<Customer> _repo;
    private readonly IValidator<Customer> _customerValidator;
    private readonly IMapper _mapper;

    public CustomerService(IBaseRepository<Customer> repo, IValidator<Customer> customerValidator, IMapper mapper)
    {
        _repo = repo;
        _customerValidator = customerValidator;
        _mapper = mapper;
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) throw new NotFoundException($"Customer {id} not found");
        return _mapper.Map<CustomerDto>(entity);
    }

    //public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    //{
    //    var entity = _mapper.Map<Customer>(dto);

    //    ValidationResult vr = await _customerValidator.ValidateAsync(entity);
    //    if (!vr.IsValid) throw new FluentValidation.ValidationException(vr.Errors);

    //    await _repo.AddAsync(entity);
    //    await _repo.SaveChangesAsync();

    //    return _mapper.Map<CustomerDto>(entity);
    //}

    public async Task<CustomerDto> CreateAsync(CustomerDto dto)
    {
        var entity = _mapper.Map<Customer>(dto);

        // ✅ فحص الدومين
        var vr = await _customerValidator.ValidateAsync(entity);
        if (!vr.IsValid) throw new ValidationException(vr.Errors);

        // ✅ فحص الـ Code يكون فريد قبل الحفظ
        var code = (entity.Code ?? string.Empty).Trim();
        if (await _repo.Query(x => x.Code == code).AnyAsync())
            throw new ValidationException(new[]
            {
            new ValidationFailure(nameof(Customer.Code), "Code must be unique")
        });

        entity.Code = code; // اختياري: خزّني النسخة المقطّعة

        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();

        return _mapper.Map<CustomerDto>(entity);
    }

    public async Task<CustomerDto> UpdateAsync(int id, CustomerDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) throw new NotFoundException($"Customer {id} not found");

        // map updates
        entity.Name = dto.Name;
        entity.PhoneNumber = dto.PhoneNumber;
        entity.Code = dto.Code;

        var vr = await _customerValidator.ValidateAsync(entity);
        if (!vr.IsValid) throw new FluentValidation.ValidationException(vr.Errors);

        await _repo.UpdateAsync(entity);
        await _repo.SaveChangesAsync();

        return _mapper.Map<CustomerDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) throw new NotFoundException($"Customer {id} not found");

        await _repo.DeleteAsync(entity);
        await _repo.SaveChangesAsync();
    }

    public async Task<List<CustomerDto>> ListAsync()
    {
        var items = _repo.Query().ToList();
        return await Task.FromResult(_mapper.Map<List<CustomerDto>>(items));
    }
}



//using CustomerTableTest.BLL;
//using CustomerTableTest.BLL.Common;
//using CustomerTableTest.Models;

//public class CustomerService : ICustomerService
//{
//    private readonly ICustomerRepository _customerRepository;

//    public CustomerService(ICustomerRepository customerRepository)
//    {
//        _customerRepository = customerRepository;
//    }

//    public async Task<List<Customer>> GetAllCustomersAsync()
//        => (await _customerRepository.GetAllAsync()).ToList();

//    public async Task<Customer> GetCustomerByIdAsync(int id)
//        => await _customerRepository.GetByIdAsync(id);

//    //public async Task<Customer> GetCustomerByCodeAsync(string code)
//    //    => await _customerRepository.GetByCodeAsync(code);

//    public async Task<ServiceResult> AddCustomerAsync(Customer customer)
//    {
//        var existing = await _customerRepository.GetByPhoneAsync(customer.PhoneNumber);
//        if (existing != null)
//            return ServiceResult.Failure("Phone already exists");

//        await _customerRepository.AddAsync(customer);
//        await _customerRepository.SaveChangesAsync();

//        return ServiceResult.Success();
//    }

//    public async Task<ServiceResult> UpdateCustomerAsync(int id, Customer customer)
//    {
//        var existing = await _customerRepository.GetByIdAsync(id);
//        if (existing == null)
//            return ServiceResult.Failure("Customer not found");

//        existing.Name = customer.Name;
//        existing.Code = customer.Code;
//        existing.PhoneNumber = customer.PhoneNumber;

//        _customerRepository.Update(existing);
//        await _customerRepository.SaveChangesAsync();

//        return ServiceResult.Success();
//    }

//    public async Task<ServiceResult> DeleteCustomerAsync(int id)
//    {
//        var existing = await _customerRepository.GetByIdAsync(id);
//        if (existing == null)
//            return ServiceResult.Failure("Customer not found");

//        _customerRepository.Delete(existing);
//        await _customerRepository.SaveChangesAsync();

//        return ServiceResult.Success();
//    }
//}
