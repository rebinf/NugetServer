using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NugetServer.Models
{
    /// <summary>
    /// Represents a registration result containing the count and items.
    /// </summary>
    [DebuggerDisplay($"Count={{{nameof(Count)}}}, Items={{{nameof(Items)}.Count}}")]
    public class RegistrationResult
    {
        /// <summary>
        /// The number of registration pages in the index.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// The array of registration pages.
        /// </summary>
        [JsonPropertyName("items")]
        public List<Registration> Items { get; set; } = new List<Registration>();
    }
}
