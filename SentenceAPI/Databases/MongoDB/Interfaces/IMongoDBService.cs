using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB.Interfaces
{
    /// <summary>
    /// This is a basic interface for working with a mongo database
    /// </summary>
    /// <typeparam name="DataType">The type of a </typeparam>
    public interface IMongoDBService<DataType> : IService
    {
        Task Insert(DataType entity);

        Task Update(DataType entity);

        Task<DataType> Get(int id);

        /// <summary>
        /// Gets all records which satisfy the given property-value dictionary
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task<DataType> Get(Dictionary<string, object> properties);

        Task Delete(int id);
    }
}
