using System.Collections.Generic;

using Domain.JsonConverters;
using Domain.Templates;

using MongoDB.Bson;

using Newtonsoft.Json;


namespace Application.Templates
{
    public class TemplateUpdateDto
    {
        [JsonProperty("templateID"), JsonConverter(typeof(ObjectIDJsonConverter))]
        public ObjectId TemplateID { get; set; }

        [JsonProperty("logo")]
        public byte[] NewLogo { get; set; }

        [JsonProperty("name")]
        public string NewName { get; set; }

        [JsonProperty("description")]
        public string NewDescription { get; set; }

        [JsonProperty("organization")]
        public string NewOrganization { get; set; }

        [JsonProperty("items")]
        public List<TemplateItem> NewItems { get; set; }

        [JsonProperty("published")]
        public bool? NewPublished { get; set; }


        public void Deconstruct(out bool? newPublished, out string newName, out string newDesc, out string newOrg,
                                out List<TemplateItem> newItems, out byte[] newLogo)
        {
            newPublished = NewPublished;
            newName = NewName;
            newDesc = NewDescription;
            newOrg = NewOrganization;
            newItems = NewItems;
            newLogo = NewLogo;
        }
    }
}