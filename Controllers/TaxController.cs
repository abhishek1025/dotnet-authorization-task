using authorization_project.utils.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authorization_project.Controllers
{
    [Route("/api/v1/taxes")]
    public class TaxController : Controller
    {
        [Authorize("TAX:CREATE")]
        [HttpPost]
        public IActionResult CreateTaxes()
        {
            return RestResponse.Ok();
        }
        
        [Authorize("TAX:READ")]
        [HttpGet]
        public IActionResult GetTaxes()
        {
            return RestResponse.Ok();
        }
        
        [Authorize("TAX:UPDATE")]
        [HttpPut]
        public IActionResult UpdateTaxes()
        {
            return RestResponse.Ok();
        }
        
        [Authorize("TAX:DELETE")]
        [HttpDelete]
        public IActionResult DeleteTaxes()
        {
            return RestResponse.Ok();
        }
    }
}
