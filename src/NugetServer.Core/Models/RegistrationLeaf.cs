using System.Diagnostics;
using System.Text.Json.Serialization;

namespace NugetServer.Models
{
    /// <summary>
    /// Registration Leaf
    /// </summary>
    [DebuggerDisplay($"Id={{{nameof(Id)}}}")]
    public class RegistrationLeaf
    {
        /// <summary>
        /// The URL to the registration leaf.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// The catalog entry containing the package metadata.
        /// </summary>
        [JsonPropertyName("catalogEntry")]
        public CatalogEntry CatalogEntry { get; set; }

        /// <summary>
        /// The URL to the package content (.nupkg).
        /// </summary>
        [JsonPropertyName("packageContent")]
        public string PackageContent { get; set; }
    }
}