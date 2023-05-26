using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugServer.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Converts a stream to a byte array.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>A byte array representation of the stream.</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
