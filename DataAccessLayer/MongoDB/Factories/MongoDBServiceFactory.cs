using DataAccessLayer.CommonInterfaces;

using System;

using Domain.KernelModels;


namespace DataAccessLayer.MongoDB.Factories
{
    public class MongoDBServiceFactory : IDatabaseFactory
    {
        public WeakReference<IDatabaseService<DataType>> GetDatabase<DataType>()
            where DataType : UniqueEntity
        {
            return new WeakReference<IDatabaseService<DataType>>(new MongoDBService<DataType>());
        }
    }
}
