using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Documents.DocumentElement.Models;
using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Documents.DocumentElement.Interface
{
    public interface IBranchService : IService
    {
        Task<DocumentElementDto> CreateNewBranchAsync(ObjectId documentElementID, string branchName, ObjectId userID);
        Task<DocumentElementDto> DeleteBranchAsync(ObjectId elementID, ObjectId branchID, ObjectId userID);
    }
}