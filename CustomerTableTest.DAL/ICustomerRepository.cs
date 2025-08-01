﻿
using CustomerTableTest.DAL.Repositories;
using CustomerTableTest.Models;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByPhoneAsync(string phone);
   // Task<Customer> GetByCodeAsync(string code);
}

//using CustomerTableTest.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomerTableTest.DAL
//{

//        public interface ICustomerRepository
//        {
//            Task<List<Customer>> GetAllAsync();
//            Task<Customer> GetByIdAsync(int id);
//            Task<Customer> GetByCodeAsync(string code);
//            Task AddAsync(Customer customer);
//            Task UpdateAsync(Customer customer);
//            Task DeleteAsync(Customer customer);
//        }
//    }

