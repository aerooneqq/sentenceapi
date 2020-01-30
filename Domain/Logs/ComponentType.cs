namespace Domain.Logs
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