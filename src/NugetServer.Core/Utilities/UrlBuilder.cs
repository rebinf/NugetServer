using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetServer.Utilities
{
    public static class UrlBuilder
    {
        public static string Combine(params string[] parts)
        {
            return string.Join("/", parts.Select(p => p.TrimStart('/').TrimEnd('/')));
        }
    }
}
