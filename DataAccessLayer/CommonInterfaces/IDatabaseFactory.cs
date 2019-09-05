using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.CommonInterfaces
{
    public interface IDatabaseFactory
    {
        WeakReference<IDatabaseService<DataType>> GetDatabase<DataType>()
            where DataType : UniqueEntity;
    }
}
