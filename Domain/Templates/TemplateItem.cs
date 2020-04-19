using System;
using System.Collections.Generic;
using Domain.DocumentStructureModels;
using Domain.KernelModels;
using MongoDB.Bson;

namespace Domain.Templates 
{
    public class TemplateItem
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public List<TemplateItem> Items { get; set; }
        public string Comment { get; set; }

        public TemplateItem() 
        {
            Items = new List<TemplateItem>();
        }

        public TemplateItem(string name, ItemType itemType) : this() 
        {
            Name = name;
            ItemType = itemType;
        }
    }
}