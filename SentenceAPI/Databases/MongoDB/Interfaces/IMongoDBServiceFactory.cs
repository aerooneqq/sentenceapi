using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.KernelInterfaces;
using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.MongoDB.Interfaces
{
    public interface IMongoDBServiceFactory : IFactory
    {
        IMongoDBService<DataType> GetService<DataType>() where DataType : UniqueEntity;

        IMongoDBServiceBuilder<DataType> GetBuilder<DataType>(IMongoDBService<DataType> mongoDBService) 
            where DataType : UniqueEntity;
    }
}
