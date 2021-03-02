using AutoMapper;
using PDM.Data.Entity.Models;
using PDM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Data.EntityMapper
{
    public sealed class MapperConfig
    {
        static MapperConfig mapperConfig = new MapperConfig();

        static IMapper mapper;
        public static MapperConfig Instance => mapperConfig;
        public static IMapper Mapper => mapper;
        private MapperConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProCustomer, Customer>();
                cfg.CreateMap<ProAddress, Address>();
                cfg.CreateMap<ProContact, Contact>();
                cfg.CreateMap<ProOrder, Order>();
                cfg.CreateMap<ProUser, User>();

                cfg.CreateMap<Customer, ProCustomer>();
                cfg.CreateMap<Address, ProAddress>();
                cfg.CreateMap<Contact, ProContact>();
                cfg.CreateMap<Order, ProOrder>();
                cfg.CreateMap<User, ProUser>();

            });
            mapper = config.CreateMapper();
        }
    }
}
