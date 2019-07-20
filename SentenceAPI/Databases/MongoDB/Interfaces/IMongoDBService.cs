using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.KernelInterfaces;

namespace SentenceAPI.Databases.MongoDB.Interfaces
{
    /// <summary>
    /// This is a basic interface for working with a mongo database
    /// </summary>
    /// <typeparam name="DataType">The type of a </typeparam>
    public interface IMongoDBService<DataType> : IDatabaseService
    {
        #region Properties
        string UserName { get; set; }
        string Password { get; set; }
        string ConnectionString { get; set; }
        string CollectionName { get; set; }
        string SupportCollectionName { get; set; }
        string SupportDocumentName { get; set; }
        IConfiguration Configuration { get; set; }
        string DatabaseName { get; set; }
        string AuthMechanism { get; set; } 
        #endregion

        #region Methods
        Task Insert(DataType entity);
        Task Update(DataType entity, IEnumerable<string> properties);
        Task<DataType> Get(long id);

        /// <summary>
        /// Gets all records which satisfy the given property-value dictionary
        /// </summary>
        Task<IEnumerable<DataType>> Get(Dictionary<string, object> properties);
        Task Delete(long id);
        #endregion
    }
}
