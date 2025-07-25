using CustomerTableTest.BLL.Common;
using CustomerTableTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerTableTest.BLL
{


    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        //Task<Customer> GetCustomerByCodeAsync(string code);

        Task<ServiceResult> AddCustomerAsync(Customer customer);
        Task<ServiceResult> UpdateCustomerAsync(int id, Customer customer);
        Task<ServiceResult> DeleteCustomerAsync(int id);
    }

    //public interface ICustomerService
    //{
    //    Task<List<Customer>> GetAllCustomersAsync();
    //    Task<Customer> GetCustomerByIdAsync(int id);
    //    Task<Customer> GetCustomerByCodeAsync(string code);
    //    Task AddCustomerAsync(Customer customer);
    //    Task UpdateCustomerAsync(Customer customer);
    //    Task DeleteCustomerAsync(Customer customer);
    //}
}
