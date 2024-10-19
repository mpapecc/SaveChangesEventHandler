using AutoMapper;
using Contacts.Api.Utils.Mappings.Abstraction;

namespace Contacts.Api.Utils.Mappings.Implementation
{
    public class CustomMap : ICustomMap
    {
        public TTo Map<TTo, TFrom>(TFrom source)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<TFrom, TTo>()).CreateMapper().Map<TFrom, TTo>(source);
        }

        public List<TTo> MapList<TTo, TFrom>(IList<TFrom> source)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<TFrom, TTo>()).CreateMapper().Map<IList<TFrom>, List<TTo>>(source);
        }
    }
}
