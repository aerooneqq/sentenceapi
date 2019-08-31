using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Filters.Interfaces
{
    public interface IFilterCollection : IDisposable
    {
        void Add(IFilter filter);
        void Remove(IFilter filter);
        void Remove(int index);
        void Clear();

        FilterDefinition<DataType> ToMongoFilter<DataType>();
    }
}
