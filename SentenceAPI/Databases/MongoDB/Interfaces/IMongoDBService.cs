using Microsoft.Extensions.Configuration;
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB.Interfaces
{
    public interface IMongoDBService<DataType> : IDatabaseService<DataType>
        where DataType : UniqueEntity
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
    }
}
