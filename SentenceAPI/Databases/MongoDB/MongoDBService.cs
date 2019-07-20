using SentenceAPI.Databases.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

using SentenceAPI.Databases.Exceptions;
using SentenceAPI.KernelModels;
using System.Reflection;

namespace SentenceAPI.Databases.MongoDB
{
    public class MongoDBService<DataType> : IMongoDBService<DataType>, IDisposable
        where DataType : UniqueEntity
    {
        #region Fields
        private MongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<DataType> mongoCollection;
        private IMongoCollection<CollectionProperties> supportMongoCollection;
        #endregion

        #region Properties
        public string AuthMechanism { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string SupportCollectionName { get; set; }
        public string SupportDocumentName { get; set; }
        public string ConnectionString { get; set; }
        public IConfiguration Configuration { get; set; }
        #endregion

        #region Constructors
        public MongoDBService() { }
        #endregion

        #region IMongoDBService<DataType> implementation
        /// <summary>
        /// Connects to the mongo database in a cloud using the connection string
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            await Task.Run(() =>
            {
                mongoClient = new MongoClient(ConnectionString);
                database = mongoClient.GetDatabase(DatabaseName);
            });
        }

        /// <summary>
        /// This method deletes the record with a given id from a mongo cloud database.
        /// </summary>
        /// <exception cref="DatabaseException">
        /// When anu exception occurs in this method
        /// </exception>
        public async Task Delete(long id)
        {
            try
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                supportMongoCollection = database.GetCollection<CollectionProperties>(SupportDocumentName);

                await CheckIfRecordWithLastID(id, supportMongoCollection);

                await mongoCollection.DeleteOneAsync(Builders<DataType>.Filter.Eq("_id", id));
            }
            catch
            {
                throw new DatabaseException("Error occured while deleting the entity");
            }
        }

        /// <summary>
        /// If we delete the record with the last id then we should decrement the "LastID"
        /// value in the SupportDatabase
        /// </summary>
        /// <param name="id">Id of recrod which is being deleted</param>
        private async Task CheckIfRecordWithLastID(long id, 
            IMongoCollection<CollectionProperties> supportCollection)
        {
            var filter = Builders<CollectionProperties>.Filter.Eq("collectionName", SupportDocumentName);
            CollectionProperties document = (await supportCollection.Find(filter).FirstAsync());

            if (document.LastID == id)
            {
                await supportCollection.UpdateOneAsync(filter, Builders<CollectionProperties>.Update.Set(
                    "lastID", id - 1));
            }
        }

        public async Task<DataType> Get(long id)
        {
            mongoCollection = database.GetCollection<DataType>(CollectionName);
            var filter = Builders<DataType>.Filter.Eq("_id", id);

            return (await mongoCollection.Find(filter).FirstAsync());
        }

        public Task<DataType> Get(Dictionary<string, object> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts one entity in a cloud mongo database.
        /// </summary>
        /// <exception cref="DatabaseException">
        /// When any exception occur
        /// </exception>
        public async Task Insert(DataType entity)
        {
            try
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                supportMongoCollection = database.GetCollection<CollectionProperties>(SupportCollectionName);

                entity.ID = await GetNewID(supportMongoCollection);

                await mongoCollection.InsertOneAsync(entity);
            }
            catch
            {
                throw new DatabaseException("Error occured while inserting the entity in a" +
                    "mongo database");
            }
        }

        /// <summary>
        /// Gets the last id for a new record, and incrementing the LastID peroperty in a support
        /// collection.
        /// </summary>
        private async Task<long> GetNewID(IMongoCollection<CollectionProperties> supportCollection)
        {
            var filter = Builders<CollectionProperties>.Filter.Eq("collectionName", SupportDocumentName);
            CollectionProperties document = (await supportCollection.Find(filter).FirstAsync());

            long lastID = document.LastID;
            await supportCollection.UpdateOneAsync(filter, Builders<CollectionProperties>.Update.Set(
                "lastID", lastID + 1));

            return lastID;
        }

        /// <summary>
        /// Tries to update the record. Only the properties which are listed in the "properties"
        /// will be updated.
        /// </summary>
        /// <exception cref="DatabaseException">
        /// Fires when any error while working with the database occurs.
        /// </exception>
        public async Task Update(DataType entity, IEnumerable<string> properties)
        {
            try
            {
                var filter = Builders<DataType>.Filter.Eq("_id", entity.ID);
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                DataType record = (await mongoCollection.Find(filter).FirstAsync());

                var updateDocument = Builders<DataType>.Update;
                foreach (string propertyName in properties)
                {
                    #warning create a better solution by using dynamic methods
                    PropertyInfo property = typeof(DataType).GetProperty(propertyName);
                    string bsonPropertyName = property.GetCustomAttribute<BsonElementAttribute>().ElementName;

                    updateDocument.Set(bsonPropertyName, property.GetValue(entity));
                }

                await mongoCollection.UpdateOneAsync(filter, updateDocument.ToBsonDocument());
            }
            catch
            {
                throw new DatabaseException("Error while updating the given record");
            }
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
