using System;
using System.Collections.Generic;
using System.Linq;
using Domain.KernelModels;
using Domain.Templates;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;


namespace Domain.DocumentStructureModels
{
    public class DocumentStructureModel : UniqueEntity
    { 
        [BsonElement("lastUpdatedAt"), JsonProperty("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        [BsonElement("parentDocumentID"), JsonProperty("parentDocumentID")]
        public ObjectId ParentDocumentID { get; set; }

        [BsonElement("items"), JsonProperty("items")]
        public List<Item> Items { get; set; }


        public static DocumentStructureModel GetNewDocumentStructure(DateTime lastUpdatedDate, ObjectId documentID,
                                                                     ObjectId userID, string documentName) 
        {
            ObjectId documentStructureID = ObjectId.GenerateNewId();

            Item rootItem = new Item() 
            {
                CreatedAt = lastUpdatedDate,
                Items = new List<Item>(),
                ItemStatus = new ItemStatus.ItemStatus()
                {
                    ItemType = ItemType.Item,
                    Accesses = new List<ItemStatus.ItemUserRole>()
                    {
                        new ItemStatus.ItemUserRole()
                        {
                            Access = ItemStatus.AccessType.CanAccess,
                            UserID = userID
                        }
                    }
                },
                Name = documentName,
                UpdatedAt = lastUpdatedDate
            };

            return new DocumentStructureModel
            {
                ID = documentStructureID,
                Items = new List<Item>() 
                {
                    rootItem
                },
                LastUpdatedAt = lastUpdatedDate,
                ParentDocumentID = documentID
            };
        }
        
        public void ApplyTemplate(List<TemplateItem> templateItems)
        {
            if (templateItems is null)
            {
                throw new ArgumentNullException("Template is null");
            }

            InitializeStructure(Items.First().Items, templateItems.First().Items);
        }

        private static void InitializeStructure(List<Item> items, List<TemplateItem> templateItems)
        {
            if (templateItems is null || templateItems.Count == 0)
            {
                return;
            }

            items.Clear();
            items.AddRange(templateItems.Select(templateItem => new Item(templateItem)));

            for (int i = 0; i < items.Count; ++i)
            {
                InitializeStructure(items[i].Items, templateItems[i].Items);
            }
        }
    }
}