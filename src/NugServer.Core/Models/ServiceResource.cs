using System.Diagnostics;
using System.Text.Json.Serialization;

namespace NugServer.Models
{
    /// <summary>
    /// Represents a versioned capability of a package source.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(Type)}}} ({{{nameof(Id)}}})")]
    public class ServiceResource
    {
        /// <summary>
        /// The URL to the resource.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// A string constant representing the resource type.
        /// </summary>
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        /// <summary>
        /// A human readable description of the resource.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}