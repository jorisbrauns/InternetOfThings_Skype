using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace T4T.Domain.Service.Controllers
{
    [Route("api/[controller]")]
    public class SkypeController : Controller
    {
        private static string _myStatus = "Service running";

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            return _myStatus;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]UpdateStatusRequest status)
        {
            if (status != null)
                _myStatus = status.Status;
        }
        
    }
    
    public class UpdateStatusRequest
    {
        public string Status { get; set; }
    }
}
