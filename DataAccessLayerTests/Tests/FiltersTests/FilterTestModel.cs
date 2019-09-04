using System;
using System.Collections.Generic;
using System.Text;
using DataAccessLayer.KernelModels;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.Tests.FiltersTests
{
    class FilterTestModel : UniqueEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [BsonElement("numbers")]
        public List<int> Numbers { get; set; }
    }
}
