using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsAPI.Features.DocumentStructure 
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class DocumentStructureController 
    {
        #region Services

        #endregion

        public DocumentStructureController()
        { 

        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentStructure([FromQuery]long documentID)
        { 
            return null;

            try 
            { 
                
            }
            catch (DatabaseException ex)
            {

            }
        }
    }
}