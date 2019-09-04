using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NUnit.Framework;

using MongoDB.Driver;

using Newtonsoft.Json;

namespace DataAccessLayer.Tests.FiltersTests
{
    [TestFixture]
    class PrepareCollectionForTests
    {
        private static Random Random => new Random();
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        private static readonly string[] names = new[]
        {
            "John",
            "AeroOne123",
            "Mike",
            "Gambit123321",
            "Mike123212312312",
            "Evgeniy",
            "Евгений", 
            "Вадим",
        };

        [Test]
        public void PrepareCollection()
        {
            var mongoClient = new MongoClient(GetConnectionString());
            var database = mongoClient.GetDatabase("SentenceDatabase");
            database.DropCollection("FilterTestCollection");
            database.CreateCollection("FilterTestCollection");
            var collection = database.GetCollection<FilterTestModel>("FilterTestCollection");

            var models = new List<FilterTestModel>();

            for (int i = 0; i < 100; i++)
            {
                models.Add(new FilterTestModel()
                {
                    ID = i,
                    Age = 2000 + i,
                    BirthDate = DateTime.Now,
                    Name = names[Random.Next(0, names.Length)],
                    Numbers = GetRandomIntList()
                });
            }

            collection.InsertMany(models);
        }

        public static string GetConnectionString()
        {
            using (FileStream fs = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), databaseConfigFile), 
                   FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd())["mongoDBConStr"];
                }
            }
        }

        private List<int> GetRandomIntList()
        {
            var numbers = new List<int>();

            for (int i = 0; i < Random.Next(100); i++)
            {
                numbers.Add(Random.Next(40));
            }

            return numbers;
        }
    }
}
