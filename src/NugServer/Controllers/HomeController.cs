using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NugServer;
using NugServer.Models;

namespace NugServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public static ServiceIndex ServiceIndex { get; set; }

        public static PackageManager PackageManager { get; set; }

        public HomeController(IOptions<NugServerOptions> options)
        {
            ServiceIndex ??= ServiceIndex.GetFullServiceIndex(options.Value.BaseUrl);
            PackageManager ??= new PackageManager(options.Value);
        }

        [HttpGet]
        [Route(RouteIds.Index)]
        public ServiceIndex Index()
        {
            return ServiceIndex;
        }

        [HttpGet]
        [Route($"{RouteIds.PackageBaseAddress}{{packageId}}/{{version}}/{{fullName}}.nupkg")]
        public FileResult GetPackage(string packageId, string version, string fullName)
        {
            var fileContent = PackageManager.GetPackage(packageId, version);

            if (fileContent == null)
            {
                return null;
            }

            return File(fileContent, "application/zip", $"{fullName}.nupkg");
        }

        [HttpGet]
        [Route("/packages/{packageId}/index.json")]
        public PackageVersions GetPackageInfo(string packageId)
        {
            return PackageManager.GetPackageVersions(packageId);
        }

        [HttpPut]
        [Route(RouteIds.PackagePublish)]
        public IActionResult PublishPackage()
        {
            if (!Request.Headers.TryGetValue("X-NuGet-ApiKey", out StringValues apiKey) || apiKey != PackageManager.Options.ApiKey)
            {
                return Unauthorized();
            }

            var file = Request.Form.Files.FirstOrDefault();

            if (file == null)
            {
                return BadRequest();
            }

            var result = PackageManager.PublishPackage(file.OpenReadStream());

            return result.Success ? Ok() : Problem(result.Message);
        }

        [HttpDelete]
        [Route($"{RouteIds.PackagePublish}/{{packageId}}/{{version}}")]
        public IActionResult DeletePackage(string packageId, string version)
        {
            if (!Request.Headers.TryGetValue("X-NuGet-ApiKey", out StringValues apiKey) || apiKey != PackageManager.Options.ApiKey)
            {
                return Unauthorized();
            }

            PackageManager.DeletePackage(packageId, version);

            return Ok();
        }

        [HttpGet]
        [Route($"{RouteIds.RegistrationsBaseUrl}{{packageId}}/index.json")]
        public RegistrationResult Registration(string packageId)
        {
            return PackageManager.Registration(packageId);
        }

        [HttpGet]
        [Route(RouteIds.SearchQueryService)]
        public QueryResult SearchQueryService([FromQuery(Name = "q")] string query)
        {
            return PackageManager.Query(query);
        }
    }
}