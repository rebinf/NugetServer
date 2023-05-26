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
    /// Represents a registration object.
    /// </summary>
    [DebuggerDisplay($"Count={{{nameof(Count)}}}, Items={{{nameof(Items)}.Count}}, Lower={{{nameof(Lower)}}}, Lower={{{nameof(Upper)}}}")]
    public class Registration
    {
        /// <summary>
        /// The URL to the registration page.
        /// </summary>
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// The number of registration leaves in the page.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// The lowest SemVer 2.0.0 version in the page (inclusive).
        /// </summary>
        [JsonPropertyName("lower")]
        public string Lower { get; set; }

        /// <summary>
        /// The highest SemVer 2.0.0 version in the page (inclusive).
        /// </summary>
        [JsonPropertyName("upper")]
        public string Upper { get; set; }

        /// <summary>
        /// The array of registration leaves and their associate metadata.
        /// </summary>
        [JsonPropertyName("items")]
        public List<RegistrationLeaf> Items { get; set; } = new List<RegistrationLeaf>();
    }
}
