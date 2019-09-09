using DataAccessLayer.Aggregations.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Aggregations
{
    public class Aggregation : IAggregation
    {
        public Type MainType { get; }
        public string LocalField { get; }
        public Dictionary<string, (string foreignKey, IEnumerable<string> requestedProperties)> ExtraCollections { get; }

        #region Constructors
        public Aggregation(Type mainType, 
                           string localField, 
                           Dictionary<string, (string foreignKey, IEnumerable<string> requestedProperties)> extraCollections)
        {
            MainType = mainType;
            LocalField = localField;
            ExtraCollections = extraCollections;
        }

        public Aggregation() { }
        #endregion
    }
}
