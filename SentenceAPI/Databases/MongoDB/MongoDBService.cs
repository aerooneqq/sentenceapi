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
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.Databases.Filters;
using SentenceAPI.Databases.Filters.Interfaces;

namespace SentenceAPI.Databases.MongoDB
{
    public class MongoDBService<DataType> : IMongoDBService<DataType>
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
        public async Task Delete(long id)
        {
            mongoCollection = database.GetCollection<DataType>(CollectionName);
            supportMongoCollection = database.GetCollection<CollectionProperties>(SupportCollectionName);

            await CheckIfRecordWithLastID(id, supportMongoCollection);

            await mongoCollection.DeleteOneAsync(Builders<DataType>.Filter.Eq("_id", id));
        }

        /// <summary>
        /// If we delete the record with the last id then we should decrement the "LastID"
        /// value in the collection.
        /// </summary>
        /// <param name="id">Id of recrod which is being deleted</param>
        private async Task CheckIfRecordWithLastID(long id,
            IMongoCollection<CollectionProperties> supportCollection)
        {
            var filter = Builders<CollectionProperties>.Filter.Eq("collectionName", SupportDocumentName);
            CollectionProperties document = (await supportCollection.Find(filter).FirstAsync());

            if (document.LastID - 1 == id)
            {
                await supportCollection.UpdateOneAsync(filter, Builders<CollectionProperties>.Update.Set(
                    "lastID", id - 1));
            }
        }

        public async Task<DataType> Get(long id)
        {
            mongoCollection = database.GetCollection<DataType>(CollectionName);
            var filter = Builders<DataType>.Filter.Eq("_id", id);
            var resList = (await mongoCollection.FindAsync(filter)).ToList();

            if (resList.Count > 1)
            {
#warning handle the exception when there is more then 1 docs with a same ID
                throw new DatabaseException("Fatal error happened.");
            }

            return resList.Count == 0 ? null : resList[0];
        }

        /// <summary>
        /// Inserts one entity in a cloud mongo database.
        /// </summary>
        public async Task Insert(DataType entity)
        {
            mongoCollection = database.GetCollection<DataType>(CollectionName);
            supportMongoCollection = database.GetCollection<CollectionProperties>(SupportCollectionName);

            entity.ID = await GetNewID(supportMongoCollection);

            await mongoCollection.InsertOneAsync(entity);
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
        /// Updates the whole entity, whoch means the replacement of the entity in the database to
        /// a given one.
        /// </summary>
        public async Task Update(DataType entity)
        {
            var filter = Builders<DataType>.Filter.Eq("_id", entity.ID);

            mongoCollection = database.GetCollection<DataType>(CollectionName);
            await mongoCollection.ReplaceOneAsync(filter, entity);
        }

        /// <summary>
        /// Tries to update the record. Only the properties which are listed in the "properties"
        /// dictionary will be updated.
        /// </summary>
        public async Task Update(DataType entity, IEnumerable<string> properties)
        {
            var filter = Builders<DataType>.Filter.Eq("_id", entity.ID);
            mongoCollection = database.GetCollection<DataType>(CollectionName);
            DataType record = (await mongoCollection.Find(filter).FirstAsync());

            foreach (string propertyName in properties)
            {
#warning create a better solution by using dynamic methods
                PropertyInfo property = typeof(DataType).GetProperty(propertyName);
                string bsonPropertyName = property.GetCustomAttribute<BsonElementAttribute>().ElementName;

                UpdateDefinition<DataType> update = Builders<DataType>.Update.Set(bsonPropertyName,
                    property.GetValue(entity));

                await mongoCollection.UpdateOneAsync(filter, update);
            }
        }

        public async Task<bool> IsCollectionExist()
        {
            List<string> collectionsList = (await database.ListCollectionNamesAsync()).ToList();
            if (collectionsList.FindIndex(n => n == CollectionName) > -1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the collection and document in a support collection.
        /// </summary>
        public async Task CreateCollection()
        {
            await database.CreateCollectionAsync(CollectionName);

            IMongoCollection<CollectionProperties> supportCollection =
                 database.GetCollection<CollectionProperties>(SupportCollectionName);

            await supportCollection.InsertOneAsync(new CollectionProperties()
            {
                CollectionName = SupportDocumentName,
                LastID = 0
            });
        }

        /// <summary>
        /// Deletes the collection and also deletes the support document of this collection
        /// in the support collection.
        /// </summary>
        public async Task DeleteCollection()
        {
            await database.DropCollectionAsync(CollectionName);

            IMongoCollection<CollectionProperties> supportCollection =
                database.GetCollection<CollectionProperties>(SupportDocumentName);

            var filter = Builders<CollectionProperties>.Filter.Eq("collectionName", SupportDocumentName);
            await supportCollection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<DataType>> Get(IFilter filter)
        {
            mongoCollection = database.GetCollection<DataType>(CollectionName);

            return (await mongoCollection.FindAsync(filter.ToMongoFilter<DataType>())).ToList();
        }

        public async Task<IEnumerable<DataType>> Get(IFilterCollection filterCollection)
        {
            mongoCollection = database.GetCollection<DataType>(CollectionName);

            return (await mongoCollection.FindAsync(filterCollection.ToMongoFilter<DataType>())).ToList();
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            mongoClient = null;
            mongoCollection = null;
            supportMongoCollection = null;
        }
        #endregion
    }
}
