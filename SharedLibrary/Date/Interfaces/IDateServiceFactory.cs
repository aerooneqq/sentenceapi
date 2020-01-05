using SharedLibrary.KernelInterfaces;


namespace SharedLibrary.Date.Interfaces
{
    public interface IDateServiceFactory : IServiceFactory
    {
        IDateService GetService();
    }
}
