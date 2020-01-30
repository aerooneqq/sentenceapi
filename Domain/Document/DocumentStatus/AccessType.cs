namespace Domain.Document.DocumentStatus
{
    public enum AccessType : byte
    {
        Read = 0,
        Write,
        ReadWrite,
        
        None,
    }
}