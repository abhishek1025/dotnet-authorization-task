using authorization_project.utils;
using authorization_project.utils.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authorization_project.Controllers
{
    [Route("/api/v1/resources")]
    [ApiController]
    [Authorize]
    public class ResourceController : Controller
    {
        [HttpGet]
        public IActionResult GetAllModules()
        {
            var resources = Enum.GetNames(typeof(ResourceEnum)).Cast<string>();
            return RestResponse.Ok(data: resources);
        }
    }
}
