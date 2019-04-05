using System.Collections.Generic;

namespace Library.API.Services.Interfaces
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSoure, TDestination>();

        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}
