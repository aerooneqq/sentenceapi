using System.IO;
using System.Threading.Tasks;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.KernelModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccessLayer.MongoDB.Interfaces
{
    public interface IGridFSService
    {
        #region Methods
        Task<ObjectId> AddFile(Stream strem, string fileName);
        Task<ObjectId> AddFile(byte[] bytes, string fileName);

        Task<byte[]> GetFile(ObjectId id);
        Task<byte[]> GetFile(string fileName);
        
        Task UpdateFile(Stream stream, ObjectId id, string newFileName);
        Task UpdateFile(byte[] bytes, ObjectId id, string newFileName);

        Task DeleteFile(ObjectId id);
        #endregion
    }
}