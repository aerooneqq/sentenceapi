using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Driver;

namespace DataAccessLayer.MongoDB
{

    public static class MongoDBConnectionManager
    {
        private static IMongoClient mongoClient;

        private static string connectionString;

        /// <summary>
        /// Gets the connection string, and sets it, if the connection string was not set before
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                if (connectionString == null)
                {
                    connectionString = value;
                }
            }
        }

        public static IMongoDatabase GetDatabase(string databaseName)
        {
            if (mongoClient == null)
            {
                mongoClient = new MongoClient(ConnectionString);
            }

            return mongoClient.GetDatabase(databaseName);
        }
    }
}
