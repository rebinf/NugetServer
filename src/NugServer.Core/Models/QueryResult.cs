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
    /// Represents a result of a query operation.
    /// </summary>
    [DebuggerDisplay($"TotalHits={{{nameof(TotalHits)}}}, Data={{{nameof(Data)}.Count}}")]
    public class QueryResult
    {
        /// <summary>
        /// The total number of matches, disregarding skip and take.
        /// </summary>
        [JsonPropertyName("totalHits")]
        public int TotalHits { get; set; }

        /// <summary>
        /// The search results matched by the request.
        /// </summary>
        [JsonPropertyName("data")]
        public List<PackageMetadata> Data { get; set; } = new List<PackageMetadata>();
    }
}
