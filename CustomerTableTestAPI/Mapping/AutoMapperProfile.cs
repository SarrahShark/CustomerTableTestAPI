using AutoMapper;
using CustomerTableTest.Models;
using CustomerTableTestAPI.Models.DTOs;


namespace CustomerTableTestAPI.Mapping
{

   
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
        }
    }

    //public class AutoMapperProfile : Profile
    //{
    //    public AutoMapperProfile()
    //    {
    //        CreateMap<Customer, CustomerDto>().ReverseMap();
    //    }
    //}
}
