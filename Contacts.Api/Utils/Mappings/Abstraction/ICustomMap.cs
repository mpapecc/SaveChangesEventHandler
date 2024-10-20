namespace SaveChangesEventHandlers.Example.Utils.Mappings.Abstraction
{
    public interface ICustomMap
    {
        TTo Map<TTo, TFrom>(TFrom source);
        List<TTo> MapList<TTo, TFrom>(IList<TFrom> source);
    }
}
