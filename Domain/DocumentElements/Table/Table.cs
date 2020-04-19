using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DocumentElements.Table
{
    public class Table : DocumentElement
    {
        public const int DefaultRowCount = 2;
        public const int DefaultColCount = 2;


        [BsonElement("cells"), JsonProperty("cells")]
        public List<List<TableCell>> Cells { get; set; }


        public static Table GetDefaultTable() 
        {
            List<List<TableCell>> cells = new List<List<TableCell>>();

            for (int i = 0; i < DefaultRowCount; ++i)
            {
                cells.Add(new List<TableCell>());
                for (int j = 0; j < DefaultColCount; ++j)
                {
                    cells[i].Add(new TableCell() { Content = "Default content"}); 
                }
            }

            return new Table()
            {
                Cells = cells,
                Name = "Enter name here..."
            };
        }
    }
}
