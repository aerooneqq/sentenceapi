using SentenceAPI.KernelModels;
using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson.Serialization.Attributes;

namespace SentenceApiTests.DatabaseTests.TestModels
{
    public class TestModel : UniqueEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("year")]
        public int Year { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("city")]
        public string City { get; set; }
    }
}
