using CustomerTableTest.BLL;
using CustomerTableTest.BLL.Common;
using CustomerTableTest.Models;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
        => (await _customerRepository.GetAllAsync()).ToList();

    public async Task<Customer> GetCustomerByIdAsync(int id)
        => await _customerRepository.GetByIdAsync(id);

    //public async Task<Customer> GetCustomerByCodeAsync(string code)
    //    => await _customerRepository.GetByCodeAsync(code);

    public async Task<ServiceResult> AddCustomerAsync(Customer customer)
    {
        var existing = await _customerRepository.GetByPhoneAsync(customer.PhoneNumber);
        if (existing != null)
            return ServiceResult.Failure("Phone already exists");

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateCustomerAsync(int id, Customer customer)
    {
        var existing = await _customerRepository.GetByIdAsync(id);
        if (existing == null)
            return ServiceResult.Failure("Customer not found");

        existing.Name = customer.Name;
        existing.Code = customer.Code;
        existing.PhoneNumber = customer.PhoneNumber;

        _customerRepository.Update(existing);
        await _customerRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteCustomerAsync(int id)
    {
        var existing = await _customerRepository.GetByIdAsync(id);
        if (existing == null)
            return ServiceResult.Failure("Customer not found");

        _customerRepository.Delete(existing);
        await _customerRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
