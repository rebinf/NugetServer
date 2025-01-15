using System.Diagnostics;
using System.Text.Json.Serialization;

namespace NugetServer.Models
{
    /// <summary>
    /// Catalog Entry
    /// </summary>
    [DebuggerDisplay($"PackageId={{{nameof(PackageId)}}}, Version={{{nameof(Version)}}}")]
    public class CatalogEntry
    {
        /// <summary>
        /// The URL to the document used to produce this object.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// The ID of the package.
        /// </summary>
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        /// <summary>
        /// The full version string after normalization.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}