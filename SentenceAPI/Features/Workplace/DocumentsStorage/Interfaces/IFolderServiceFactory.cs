using SentenceAPI.Features.Workplace.DocumentsStorage.Services;
using SentenceAPI.KernelInterfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces
{
    interface IFolderServiceFactory : IServiceFactory
    {
        IFolderService GetFolderService();

        IFileService GetFileSystem();
    }
}
