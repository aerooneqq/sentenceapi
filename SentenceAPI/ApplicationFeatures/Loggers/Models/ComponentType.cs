namespace SentenceAPI.ApplicationFeatures.Loggers.Models
{
    public enum ComponentType : byte
    {
        Service = 0,
        Controller,
        Factory,
        Validator,
        Middleware,
        
        Undefined
    }
}