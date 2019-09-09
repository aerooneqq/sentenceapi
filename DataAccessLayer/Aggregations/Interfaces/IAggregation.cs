using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Aggregations.Interfaces
{
    public interface IAggregation
    {
        Type MainType { get; }
        string LocalField { get; }

        Dictionary<string, (string foreignKey, IEnumerable<string> requestedProperties)> ExtraCollections { get; }
    }
}
