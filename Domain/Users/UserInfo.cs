﻿using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Domain.KernelModels;
using Domain.Extensions;
using Domain.Attributes;
using MongoDB.Bson;


namespace Domain.Users
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

        [BsonElement("isAccountDeleted"), JsonProperty("isAccountDeleted")]
        public bool IsAccountDeleted { get; set; }
        #endregion

        #region Roles
        [BsonElement("roles"), JsonProperty("roles")]
        public IEnumerable<Role> Roles { get; set; }
        #endregion

        #region Project
        [BsonElement("userRelatedProjects"), JsonProperty("userRelatedProjects")]
        public List<ObjectId> UserRelatedProjects { get; set; }

        [BsonElement("userInvitedProjects"), JsonProperty("userInvitedProjects")]
        public List<ObjectId> UserInvitedProjects { get; set; }
        #endregion

        #region Constructors

        public UserInfo() {}

        public UserInfo(Dictionary<string, object> propertiesValues)
        {
            foreach ((string propName, object value) in propertiesValues)
            {
                object newValue = value;

                if (propName == "careerStages")
                {
                    newValue = GetCareerStagesFromJArray((JArray) value);
                }

                PropertyInfo property = typeof(UserInfo).GetPropertyFromJsonName(propName);
                property.SetValue(this, newValue is JArray array ? array.ToObject(property.PropertyType) : newValue);
            }
        }

        private List<CareerStage> GetCareerStagesFromJArray(JArray jArray)
        {
            return (from JObject jObject in jArray
                select new CareerStage()
                {
                    Company = jObject.GetValue("company").ToObject<string>(),
                    Description = jObject.GetValue("description").ToObject<string>(),
                    FinishYear = jObject.GetValue("finishYear").ToObject<int>(),
                    StartYear = jObject.GetValue("startYear").ToObject<int>(),
                    Job = jObject.GetValue("job").ToObject<string>(),
                }).ToList();
        }

        #endregion

        #region Static factories

        /// <summary>
        /// Returns default user: user with given email and password.
        /// Roles: SentenceAPIConsumer, DocumentsAPIConsumer
        /// Assuming that email and password already validated
        /// </summary>
        public static UserInfo GetDefaultUser(string email, string password) =>
            new UserInfo()
            {
                Roles = new[] { Role.SentenceAPI, Role.DocumentsAPI},
                Email = email,
                Password = password,
                ID = ObjectId.GenerateNewId(),
                #warning Replace this to false
                IsAccountVerified = true,
                IsAccountDeleted = false,
                UserInvitedProjects = new List<ObjectId>(),
                UserRelatedProjects = new List<ObjectId>(),
                CareerStages = new List<CareerStage>(),
            };

        #endregion
    }
}