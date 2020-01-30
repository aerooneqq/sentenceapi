using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Domain.KernelModels
{
    public class UniqueEntity
    {
        [BsonId, BsonElement("_id")]
        public ObjectId ID { get; set; }

        public UniqueEntity()
        {
            ID = ObjectId.GenerateNewId();
        }
    }
}
