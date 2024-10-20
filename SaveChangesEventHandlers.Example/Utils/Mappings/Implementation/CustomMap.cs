using AutoMapper;
using SaveChangesEventHandlers.Example.Utils.Mappings.Abstraction;

namespace SaveChangesEventHandlers.Example.Utils.Mappings.Implementation
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
