using System;

using Domain.KernelModels;


namespace DataAccessLayer.CommonInterfaces
{
    public interface IDatabaseFactory
    {
        WeakReference<IDatabaseService<DataType>> GetDatabase<DataType>()
            where DataType : UniqueEntity;
    }
}
