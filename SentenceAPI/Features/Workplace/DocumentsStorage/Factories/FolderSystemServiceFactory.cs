using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Factories
{
    class FolderSystemServiceFactory : IFolderServiceFactory
    {
        public IFileService GetFileSystem()
        {
            return new FileService();
        }

        public IFolderService GetFolderService()
        {
            return new FolderService();
        }
    }
}
