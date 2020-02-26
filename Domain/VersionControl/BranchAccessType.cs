namespace Domain.VersionControl
{
    public enum BranchAccessType : byte
    {
        Read = 0, 
        Write,
        ReadWrite,
        NoAccess,
    }
}