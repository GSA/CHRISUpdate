using AutoMapper;
using AutoMapper.Data;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    internal class HRMapper
    {
        private MapperConfiguration lookupConfig;
        private MapperConfiguration saveConfig;

        public void CreateLookupConfig()
        {
            lookupConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.AllowNullCollections = true;
            });

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.AddDataReaderMapping();
            //    cfg.AllowNullCollections = true;

            //    //cfg.CreateMap<Investigation_Lookup, Lookup>()
            //    //.ForMember(dest => dest.investigation_lookup, opt => opt.MapFrom(src => src.Tier))
            //    //.ForMember(dest => dest.investigation_lookup, opt => opt.MapFrom(src => src.Code));
            //});
        }

        public void CreateSaveConfig()
        {
            saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.AllowNullCollections = true;
                cfg.CreateMap<Employee, Person>().ForMember(dest => dest.SocialSecurityNumber, opt => opt.Ignore());
            });
        }

        public IMapper CreateLookupMapping()
        {
            return lookupConfig.CreateMapper();
        }

        public IMapper CreateSaveMapping()
        {
            return saveConfig.CreateMapper();
        }
    }
}