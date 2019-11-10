using System.IO;
using System.Threading.Tasks;

using DataAccessLayer.MongoDB.Interfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;

using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;

namespace DataAccessLayer.MongoDB
{
    public class GridFSService : IGridFSService
    {
        #region Fields
        private IGridFSBucket gridFS;
        private IMongoDatabase database;
        public IConfigurationBuilder configurationBuilder;
        #endregion

        public GridFSService(IMongoDatabase database) 
        {
            this.database = database;
            gridFS = new GridFSBucket(database);
        }

        public Task<ObjectId> AddFile(Stream stream, string fileName)
        {
            return Task.Run(() => 
            { 
                ObjectId id = ObjectId.GenerateNewId();

                gridFS.UploadFromStream(id, fileName, stream);

                return id;
            });
        }

        public Task<ObjectId> AddFile(byte[] bytes, string fileName)
        {
            return AddFile(new MemoryStream(bytes), fileName);
        }

        public Task DeleteFile(ObjectId id)
        {
            return Task.Run(() =>
            { 
                gridFS.Delete(id);
            });
        }

        public Task<byte[]> GetFile(ObjectId id)
        {
            return Task.Run(() => 
            {
                return gridFS.DownloadAsBytes(id);
            });
        }

        public Task<byte[]> GetFile(string fileName)
        {
            return Task.Run(() => 
            { 
                return gridFS.DownloadAsBytesByName(fileName);
            });
        }

        public Task UpdateFile(Stream stream, ObjectId id, string newFileName)
        {
            return Task.Run(() => 
            { 
                gridFS.Delete(id);
                gridFS.UploadFromStream(id, newFileName, stream);
            });
        }

        public Task UpdateFile(byte[] bytes, ObjectId id, string newFileName)
        {
            return UpdateFile(new MemoryStream(bytes), id, newFileName);
        }
    }
}