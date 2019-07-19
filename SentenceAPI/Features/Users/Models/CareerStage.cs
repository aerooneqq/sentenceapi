using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Users.Models
{
    /// <summary>
    /// This class represnets one of the Career stage of a employee
    /// </summary>
    public class CareerStage
    {
        public string Company { get; set; }
        public string Job { get; set; }
        public DateTime StartYear { get; set; }
        public DateTime FinishYear { get; set; }
    }
}
