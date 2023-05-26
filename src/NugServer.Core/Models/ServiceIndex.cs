using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NugServer.Models
{
    /// <summary>
    /// A JSON document that is the entry point for a NuGet package source and allows a client implementation to discover the package source's capabilities.
    /// </summary>
    public class ServiceIndex
    {
        /// <summary>
        /// Service version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "3.0.0";

        /// <summary>
        /// Resources supported by this package source.
        /// </summary>
        [JsonPropertyName("resources")]
        public List<ServiceResource> Resources { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceIndex"/> class.
        /// </summary>
        public ServiceIndex()
        {
            Resources = new List<ServiceResource>();
        }

        /// <summary>
        /// Gets the full index of the service.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        public static ServiceIndex GetFullServiceIndex(string baseUrl)
        {
            var index = new ServiceIndex();

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.PackageBaseAddress}",
                Type = "PackageBaseAddress/3.0.0",
                Comment = $"Base URL of where NuGet packages are stored, in the format {baseUrl}/v3-flatcontainer/{{id-lower}}/{{version-lower}}/{{id-lower}}.{{version-lower}}.nupkg."
            });

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.SearchQueryService}",
                Type = "SearchQueryService",
                Comment = "Search endpoint for the NuGet Search service."
            });

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.SearchQueryService}",
                Type = "SearchQueryService/3.0.0-beta",
                Comment = "Search endpoint for the NuGet Search service."
            });

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.SearchQueryService}",
                Type = "SearchQueryService/3.0.0-rc",
                Comment = "Search endpoint for the NuGet Search service."
            });

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.SearchQueryService}",
                Type = "SearchQueryService/3.5.0",
                Comment = "Search endpoint for the NuGet Search service."
            });

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.RegistrationsBaseUrl}",
                Type = "RegistrationsBaseUrl",
                Comment = "Base URL of where NuGet package registration info is stored."
            });

            index.Resources.Add(new ServiceResource
            {
                Id = $"{baseUrl}{RouteIds.PackagePublish}",
                Type = "PackagePublish/2.0.0",
                Comment = "Package publishing endpoint."
            });

            return index;
        }
    }
}
