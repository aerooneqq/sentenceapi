﻿using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using DataAccessLayer.Interfaces.Aggregations;
using DataAccessLayer.KernelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.CommonInterfaces
{
    public interface IDatabaseService<DataType> : IDisposable 
        where DataType : UniqueEntity
    {
        #region Properties
        IDatabaseConfiguration Configuration { get; }
        #endregion

        #region Methods
        Task Connect();

        Task<bool> DoesCollectionExist();

        Task CreateCollection();

        Task DeleteCollection();

        Task Insert(DataType entity);

        Task Update(DataType entity, IEnumerable<string> properties);

        Task Update(DataType entity);

        Task<IEnumerable<DataType>> Get(IFilter filter);

        Task<IEnumerable<DataType>> Get(IFilterCollection filterCollection);
        Task<IEnumerable<dynamic>> GetCombined(IFilter filter, string localField,
            params (Type entityType, string foreignField, IEnumerable<string> requestedFields)[] extraEntitiesTypes);


        Task Delete(IFilter filter);
        #endregion
    }
}