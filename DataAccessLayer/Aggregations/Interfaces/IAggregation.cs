using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Aggregations.Interfaces
{
    /// <summary>
    /// This interface represents the data which is needed to aggregate + lookup (in mongo db)
    /// or to inner join in the SQL
    /// </summary>
    public interface IAggregation
    {
        /// <summary>
        /// The main type from which the aggregation starts
        /// </summary>
        Type MainType { get; }

        /// <summary>
        /// The local field which is used as a "primary" key
        /// </summary>
        string LocalField { get; }

        /// <summary>
        /// The dictionary which represents the collections which will be united in the request
        /// 
        /// Keys:
        /// The keys are the collections names
        /// 
        /// Values:
        /// foreignKey is a foreign key in the extra collection
        /// requestedProperties are the properties which will be included in the reponse
        /// 
        /// </summary>
        Dictionary<string, (string foreignKey, IEnumerable<string> requestedProperties)> ExtraCollections { get; }
    }
}
