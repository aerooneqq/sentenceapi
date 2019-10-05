using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

using DataAccessLayer.Exceptions;
using DataAccessLayer.KernelModels;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.MongoDB.Interfaces;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Aggregations.Interfaces;
using DataAccessLayer.Aggregations;

namespace DataAccessLayer.MongoDB
{
    internal class MongoDBService<DataType> : IMongoDBService<DataType>
        where DataType : UniqueEntity
    {
        #region Static fields
        private static readonly string collectionPostfix = "Collection";
        private static readonly string supportDocumentPostfix = "Support";
        private static readonly string supportCollectionName = "SupportCollection";
        #endregion

        #region Fields
        private IMongoDatabase database;
        private IMongoCollection<DataType> mongoCollection;
        private IMongoCollection<CollectionProperties> supportMongoCollection;
        #endregion

        #region Properties
        public string CollectionName { get; set; }
        public string SupportCollectionName { get; set; }
        public string SupportDocumentName { get; set; }
        public IDatabaseConfiguration Configuration { get; set; }
        #endregion

        #region Constructors
        public MongoDBService()
        {
            CollectionName = typeof(DataType).Name + collectionPostfix;
            SupportDocumentName = typeof(DataType).Name + collectionPostfix + supportDocumentPostfix;
            SupportCollectionName = supportCollectionName;

            Configuration = new MongoDatabaseConfiguration();
        }
        #endregion

        #region IMongoDBService<DataType> implementation
        /// <summary>
        /// Connects to the mongo database in a cloud using the connection string
        /// </summary>
        /// <returns></returns>
        public Task Connect()
        {
            return Task.Run(() =>
            {
                MongoDBConnectionManager.ConnectionString = Configuration.ConnectionString;
                database = MongoDBConnectionManager.GetDatabase(Configuration.DatabaseName);
            });
        }

        /// <summary>
        /// This method deletes the record with a given id from a mongo cloud database.
        /// </summary>
        public Task Delete(IFilter filter)
        {
            return Task.Run(() =>
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                supportMongoCollection = database.GetCollection<CollectionProperties>(SupportCollectionName);

                mongoCollection.DeleteMany(filter.ToMongoFilter<DataType>());
            });
        }

        public Task<DataType> Get(long id)
        {
            return Task.Run(() =>
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                var filter = Builders<DataType>.Filter.Eq("_id", id);
                var resList = mongoCollection.Find(filter).ToList();

                if (resList.Count > 1)
                {
#warning handle the exception when there is more then 1 docs with a same ID
                    throw new DatabaseException("Fatal error happened.");
                }

                return resList.Count == 0 ? null : resList[0];
            });

        }

        /// <summary>
        /// Inserts one entity in a cloud mongo database.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If the given entity is not of a type <DataType>
        /// </exception>
        public Task Insert(DataType entity)
        {
            if (!(entity is DataType))
            {
                throw new ArgumentException($"The entity is not of type {typeof(DataType).Name}");
            }

            return Task.Run(() =>
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                supportMongoCollection = database.GetCollection<CollectionProperties>(SupportCollectionName);

                entity.ID = GetNewID(supportMongoCollection);

                mongoCollection.InsertOneAsync(entity);
            });
        }

        /// <summary>
        /// Gets the last id for a new record, and incrementing the LastID peroperty in a support
        /// collection.
        /// </summary>
        private long GetNewID(IMongoCollection<CollectionProperties> supportCollection)
        {
            var filter = Builders<CollectionProperties>.Filter.Eq("collectionName", SupportDocumentName);
            CollectionProperties document = supportCollection.Find(filter).First();

            long lastID = document.LastID;
            supportCollection.UpdateOne(filter, Builders<CollectionProperties>.Update.Set(
                "lastID", lastID + 1));

            return lastID;
        }

        /// <summary>
        /// Updates the whole entity, whoch means the replacement of the entity in the database to
        /// a given one.
        /// </summary>
        public Task Update(DataType entity)
        {
            if (!(entity is DataType))
            {
                throw new ArgumentException($"The entity is not of type {typeof(DataType).Name}");
            }

            return Task.Run(() =>
            {
                var filter = Builders<DataType>.Filter.Eq("_id", entity.ID);

                mongoCollection = database.GetCollection<DataType>(CollectionName);
                mongoCollection.ReplaceOne(filter, entity);
            });
        }

        /// <summary>
        /// Tries to update the record. Only the properties which are listed in the "properties"
        /// dictionary will be updated.
        /// </summary>
        public Task Update(DataType entity, IEnumerable<string> properties)
        {
            return Task.Run(() =>
            {
                var filter = Builders<DataType>.Filter.Eq("_id", entity.ID);
                mongoCollection = database.GetCollection<DataType>(CollectionName);
                DataType record = mongoCollection.Find(filter).First();

                foreach (string propertyName in properties)
                {
                    #warning create a better solution by using dynamic methods
                    PropertyInfo property = typeof(DataType).GetProperty(propertyName);
                    string bsonPropertyName = property.GetCustomAttribute<BsonElementAttribute>().ElementName;

                    UpdateDefinition<DataType> update = Builders<DataType>.Update.Set(bsonPropertyName,
                        property.GetValue(entity));

                    mongoCollection.UpdateOne(filter, update);
                }
            });
        }

        public Task<bool> DoesCollectionExist()
        {
            return Task.Run(() =>
            {
                List<string> collectionsList = database.ListCollectionNames().ToList();
                if (collectionsList.FindIndex(n => n == CollectionName) > -1)
                {
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Creates the collection and document in a support collection.
        /// </summary>
        public Task CreateCollection()
        {
            return Task.Run(() =>
            {
                database.CreateCollection(CollectionName);

                IMongoCollection<CollectionProperties> supportCollection =
                     database.GetCollection<CollectionProperties>(SupportCollectionName);

                supportCollection.InsertOne(new CollectionProperties()
                {
                    CollectionName = SupportDocumentName,
                    LastID = 0
                });
            });
        }

        /// <summary>
        /// Deletes the collection and also deletes the support document of this collection
        /// in the support collection.
        /// </summary>
        public Task DeleteCollection()
        {
            return Task.Run(() =>
            {
                database.DropCollection(CollectionName);

                IMongoCollection<CollectionProperties> supportCollection =
                    database.GetCollection<CollectionProperties>(SupportDocumentName);

                var filter = Builders<CollectionProperties>.Filter.Eq("collectionName", SupportDocumentName);
                supportCollection.DeleteOne(filter);
            });
        }

        public Task<IEnumerable<DataType>> Get(IFilter filter)
        {
            return Task.Run(() =>
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);

                return mongoCollection.Find(filter.ToMongoFilter<DataType>()).ToEnumerable();
            });
        }

        /// <summary>
        /// Performs an aggregate + lookup operations in mongo db. In other words this method unites collections,
        /// depending on the "Primary" and "Foreign" keys, which are localField and first tuple value in the
        /// extra Collections dictionaty,
        /// and returns the result of this unite.
        /// </summary>
        /// <param name="filter">This filter is used to select records in the MAIN collection</param>
        /// <param name="localField">
        /// This is the "Primary" key in the main collection, for instance,
        /// the property UserID in the UserFeed entity.
        /// </param>
        /// <param name="extraEntitiesTypes">
        /// The types of the entities, with which we want to aggregate the main collection. From this types
        /// the extraCollections dictionary is obtained.
        /// </param>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> GetCombined(IFilter filter, string localField,
            params (Type entityType, string foreignField, IEnumerable<string> requestedFields)[] extraEntitiesTypes)
        {
            return Task.Run(() =>
            {
                mongoCollection = database.GetCollection<DataType>(CollectionName);

                Dictionary<string, (string, IEnumerable<string>)> extraCollections =
                    new Dictionary<string, (string, IEnumerable<string>)>();

                foreach ((Type entityType, string foreignField, IEnumerable<string> requestedFields) in extraEntitiesTypes)
                {
                    extraCollections.Add(entityType.Name + collectionPostfix, (foreignField, requestedFields));
                }

                IAggregationFilter aggregationFilter = new AggregationFilter(
                    new Aggregation
                    (
                        typeof(DataType),
                        localField,
                        extraCollections
                    ));

                var pipeline = new List<BsonDocument>()
                {
                    new BsonDocument("$match", filter.ToMongoBsonDocument())
                };

                pipeline.AddRange(aggregationFilter.ToBsonDocument());

                return mongoCollection.Aggregate<dynamic>(pipeline).ToEnumerable();
            });
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            mongoCollection = null;
            supportMongoCollection = null;
        }
        #endregion
    }
}
