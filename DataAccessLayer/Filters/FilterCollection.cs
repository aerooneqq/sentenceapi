using MongoDB.Driver;
using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.Filters.Interfaces;

namespace DataAccessLayer.Filters
{
    public class FilterCollection : IFilterCollection
    {
        private List<IFilter> filters;

        #region Constructors
        public FilterCollection()
        {
            filters = new List<IFilter>();
        }

        public FilterCollection(IEnumerable<IFilter> filters)
        {
            this.filters = filters.ToList();
        }
        #endregion

        #region IFilterCollection implementation
        public void Add(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException();
            }

            filters.Add(filter);
        }

        public void Clear()
        {
            filters.Clear();
        }

        public void Remove(IFilter filter)
        {
            filters.Remove(filter);
        }

        public void Remove(int index)
        {
            filters.RemoveAt(index);
        }

        public FilterDefinition<DataType> ToMongoFilter<DataType>()
        {
            if (filters == null || filters.Count == 0)
            {
                return null;
            }

            FilterDefinition<DataType> finalFilter = filters[0].ToMongoFilter<DataType>();

            for (int i = 1; i < filters.Count; i++)
            {
                finalFilter = Builders<DataType>.Filter.And(finalFilter, filters[i].ToMongoFilter<DataType>());
            }

            return finalFilter;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            filters = null;
        }
        #endregion
    }
}
