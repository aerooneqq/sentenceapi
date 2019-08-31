using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.MongoDB.Interfaces;
using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
