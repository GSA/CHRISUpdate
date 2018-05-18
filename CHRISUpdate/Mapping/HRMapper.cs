using AutoMapper;
using AutoMapper.Data;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    internal class HRMapper
    {
        private MapperConfiguration lookupConfig;
        private MapperConfiguration dataConfig;

        public void CreateLookupConfig()
        {
            lookupConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.AllowNullCollections = true;
            });
        }

        public void CreateDataConfig()
        {
            dataConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.AllowNullCollections = true;

                cfg.CreateMap<Employee, Person>().ReverseMap();
                cfg.CreateMap<Employee, Address>().ReverseMap();
                cfg.CreateMap<Employee, Birth>().ReverseMap();
                cfg.CreateMap<Employee, Investigation>().ReverseMap();
                cfg.CreateMap<Employee, Emergency>().ReverseMap();
                cfg.CreateMap<Employee, Position>().ReverseMap();
                cfg.CreateMap<Employee, Phone>().ReverseMap();

                //cfg.CreateMap<Employee, Person>().ForMember(dest => dest.SocialSecurityNumber, opt => opt.Ignore());
            });
        }

        public IMapper CreateLookupMapping()
        {
            return lookupConfig.CreateMapper();
        }

        public IMapper CreateDataMapping()
        {
            return dataConfig.CreateMapper();
        }
    }
}