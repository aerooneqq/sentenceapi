using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.MongoDB.Interfaces
{
    public interface IMongoDBService<DataType> : IDatabaseService<DataType>
        where DataType : UniqueEntity
    {
        #region Properties
        string CollectionName { get; set; }
        string SupportCollectionName { get; set; }
        string SupportDocumentName { get; set; }
        #endregion

        #region Bridges
        IGridFSService GridFS { get; }
        #endregion
    }
}
