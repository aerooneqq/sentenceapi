using Domain.KernelModels;


namespace DocumentsAPI.Models.DocumentElements.Table
{
    public class TableCell : UniqueEntity
    {
        public string Content { get; set; }
    }
}
