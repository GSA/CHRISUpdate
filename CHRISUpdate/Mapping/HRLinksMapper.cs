using AutoMapper;
using AutoMapper.Data;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    public class HRLinksMapper
    {
        private MapperConfiguration config;

        public void CreateMappingConfig()
        {
            config = new MapperConfiguration(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.CreateMap<Employee, Person>().ForMember(dest => dest.SocialSecurityNumber, opt => opt.Ignore());
            });
        }

        public IMapper CreateMapping()
        {
            return config.CreateMapper();
        }
    }
}