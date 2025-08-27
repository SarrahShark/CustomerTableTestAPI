using AutoMapper;
using CustomerTableTest.Models;
using CustomerTableTest.Models.DTOs;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<CustomerDto, Customer>();
        //CreateMap<UpdateCustomerDto, Customer>();
    }
}
