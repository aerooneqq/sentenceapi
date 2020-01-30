using DataAccessLayer.CommonInterfaces;

using Domain.KernelModels;


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
