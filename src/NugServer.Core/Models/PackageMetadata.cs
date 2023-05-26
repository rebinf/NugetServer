using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NugServer.Models
{
    /// <summary>
    /// A package metadata class.
    /// </summary>
    [DebuggerDisplay($"Id={{{nameof(Id)}}}, Version={{{nameof(Version)}}}")]
    public class PackageMetadata
    {
        /// <summary>
        /// The ID of the matched package.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [JsonPropertyName("@type")]
        public string Type { get; set; } = "Package";

        /// <summary>
        /// The ID of the matched package.
        /// </summary>
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        /// <summary>
        /// The full SemVer 2.0.0 version string of the package (could contain build metadata).
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// All of the versions of the package matching the prerelease parameter.
        /// </summary>
        [JsonPropertyName("versions")]
        public List<PackageVersion> Versions { get; set; } = new List<PackageVersion>();

        /// <summary>
        /// The package types defined by the package author (added in SearchQueryService/3.5.0).
        /// </summary>
        public List<PackageType> PackageTypes { get; set; } = new List<PackageType>() { new PackageType() { Name = "Package" } };
    }
}
