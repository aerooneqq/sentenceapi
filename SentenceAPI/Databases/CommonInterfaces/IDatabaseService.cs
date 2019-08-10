using SentenceAPI.Databases.Filters;
using SentenceAPI.Databases.Filters.Interfaces;
using SentenceAPI.KernelInterfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.CommonInterfaces
{
    public interface IDatabaseService<DataType> : IService, IDisposable 
        where DataType : UniqueEntity
    {
        Task Connect();

        Task CreateCollection();

        Task DeleteCollection();

        Task Insert(DataType entity);

        Task Update(DataType entity, IEnumerable<string> properties);

        Task Update(DataType entity);

        Task<IEnumerable<DataType>> Get(IFilter filter);

        Task<IEnumerable<DataType>> Get(IFilterCollection filterCollection);

        Task Delete(long id);
    }
}
