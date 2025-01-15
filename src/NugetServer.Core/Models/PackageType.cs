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
    /// Package Type
    /// </summary>
    [DebuggerDisplay($"Name={{{nameof(Name)}}}")]
    public class PackageType
    {
        /// <summary>
        /// The name of the package type.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Package";
    }
}
