using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NugServer.Models
{
    /// <summary>
    /// Represents a package version.
    /// </summary>
    public class PackageVersion
    {
        /// <summary>
        /// The absolute URL to the associated registration leaf.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// The full SemVer 2.0.0 version string of the package (could contain build metadata).
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The number of downloads for this specific package version.
        /// </summary>
        [JsonPropertyName("downloads")]
        public int Downloads { get; set; } = 0;
    }
}
