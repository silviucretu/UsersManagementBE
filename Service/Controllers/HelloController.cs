using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [Authorize]
        [HttpGet("{text}")]
        public ActionResult<string> Get(string text)
        {
            var roles = string.Join(",",User.FindAll("role").Select(r => r.Value));
            var tenant = User.FindFirst("tenant")?.Value;

            return new JsonResult($"{tenant} {roles}");
        }
    }
}