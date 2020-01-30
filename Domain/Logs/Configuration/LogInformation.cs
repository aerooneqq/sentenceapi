namespace Domain.Logs.Configuration
{
    public enum LogInformation : byte
    {
        Date,
        Time,
        DateTime,
        LogLevel, 
        Message,
        Place,
        ObjectJSONData,
        ObjectXMLData,
        ObjectBINARYData,

        Undefined
    }
}