using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

using Newtonsoft.Json;

using MongoDB.Bson.Serialization.Attributes;

using SentenceAPI.Attributes;
using SentenceAPI.Extensions;

using DataAccessLayer.KernelModels;
using Newtonsoft.Json.Linq;

namespace SentenceAPI.Features.Users.Models
{
    public class UserInfo : UniqueEntity
    {
        #region Authentication 
        [BsonElement("login"), JsonProperty("login")]
        public string Login { get; set; }

        [BsonElement("email"), JsonProperty("email")]
        public string Email { get; set; }

        [BsonElement("password"), JsonIgnore, Secret]
        public string Password { get; set; }
        #endregion

        #region User data (name + country + photo)
        [BsonElement("name"), JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("surname"), JsonProperty("surname")]
        public string Surname { get; set; }

        [BsonElement("middleName"), JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [BsonElement("country"), JsonProperty("country")]
        public string Country { get; set; }

        [BsonElement("city"), JsonProperty("city")]
        public string City { get; set; }

        [BsonElement("photo"), JsonProperty("photo")]
        public byte[] Photo { get; set; }
        #endregion

        [BsonElement("birthDate"), JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }

        #region Career
        [BsonElement("careerStages"), JsonProperty("careerStages")]
        public List<CareerStage> CareerStages { get; set; }
        #endregion

        #region System properties
        [BsonElement("isAccountVerified"), JsonProperty("isAccountVerified")]
        public bool IsAccountVerified { get; set; }
        #endregion

        #region Constructors
        public UserInfo() { }

        public UserInfo(Dictionary<string, object> propertiesValues)
        {
            PropertyInfo[] properties = typeof(UserInfo).GetTypeInfo().GetProperties();

            foreach ((string propName, object value) in propertiesValues)
            {
                var newValue = value;

                if (propName == "careerStages")
                {
                    newValue = GetCareerStagesFromJArray((JArray)value);
                }

                PropertyInfo property = typeof(UserInfo).GetPropertyFromJsonName(propName);
                property.SetValue(this, ((JArray)newValue).ToObject(property.PropertyType));
            }
        }

        private List<CareerStage> GetCareerStagesFromJArray(JArray jArray)
        {
            List<CareerStage> careerStages = new List<CareerStage>();

            foreach (JObject jObject in jArray)
            {
                careerStages.Add(new CareerStage()
                {
                    Company = jObject.GetValue("company").ToObject<string>(),
                    Description = jObject.GetValue("description").ToObject<string>(),
                    FinishYear = jObject.GetValue("finishYear").ToObject<int>(),
                    StartYear = jObject.GetValue("startYear").ToObject<int>(),
                    Job = jObject.GetValue("job").ToObject<string>(),
                });
            }

            return careerStages;
        }
        #endregion
    }
}
