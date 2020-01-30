using DocumentsAPI.Models.VersionControl;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Domain.KernelModels;


namespace Domain.DocumentElements
{
    public class DocumentElementWrapper : UniqueEntity
    {
        [BsonElement("parentDocumentID"), JsonProperty("parentDocumentID")]
        public ObjectId ParentDocumentID { get; set; }

        [BsonElement("parentItemID"), JsonProperty("parentItemID")]
        public ObjectId ParentItemID { get; set; }

        [BsonElement("creatorID"), JsonProperty("creatorID")]
        public ObjectId CreatorID { get; set; }

        [BsonElement("versionsCount"), JsonProperty("versionsCount")]
        public int VersionsCount { get; set; }

        [BsonElement("documentElementType"), JsonProperty("documentElementType")]
        public DocumentElementType Type { get; set; }

        /// <summary>
        /// The versions of the document element
        /// </summary>
        [BsonElement("frames"), JsonIgnore]
        public List<Frame> Frames { get; set; }

        /// <summary>
        /// The initial state of the doucment element. The current state is obtained from the initial state,
        /// by applying the Changes listed in Frames property.
        /// </summary>
        [BsonElement("documentElementRaw"), JsonIgnore]
        public byte[] DocumentElementRaw { get; set; }
        
        /// <summary>
        /// Returns the string JSON representation of the document element, which is obtained from the DocumentElementRaw.
        /// </summary>
        [BsonIgnore, JsonProperty("documentElement")]
        public string DocumentElement
        {
            get
            {
                DocumentElement documentElement = (DocumentElement)new BinaryFormatter().
                    Deserialize(new MemoryStream(DocumentElementRaw));

                switch (Type)
                {
                    case DocumentElementType.Image:
                        return JsonConvert.SerializeObject((Image.Image)documentElement);

                    case DocumentElementType.Paragraph:
                        return JsonConvert.SerializeObject((Paragraph.Paragraph)documentElement);

                    case DocumentElementType.NumberedList:
                        return JsonConvert.SerializeObject((NumberedList.NumberedList)documentElement);

                    case DocumentElementType.Table:
                        return JsonConvert.SerializeObject((Table.Table)documentElement);

                    default:
                        throw new ArgumentException("Invalid document element type");
                }
            }
        }
    }
}
