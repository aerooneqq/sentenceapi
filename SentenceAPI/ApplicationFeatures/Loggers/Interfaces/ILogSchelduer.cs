using SentenceAPI.ApplicationFeatures.Loggers.Models;

namespace SentenceAPI.ApplicationFeatures.Loggers.Interfaces
{
    public interface ILogSchelduer
    {
        void PutLogToQueue(Log log);
    }
}