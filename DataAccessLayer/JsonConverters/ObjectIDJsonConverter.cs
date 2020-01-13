using System;

using MongoDB.Bson;

using Newtonsoft.Json;


namespace DataAccessLayer.JsonConverters
{
    public class ObjectIDJsonConverter : JsonConverter<ObjectId>
    {
        public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, 
                                          bool hasExistingValue, JsonSerializer serializer)
        {
            return ObjectId.Parse((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}