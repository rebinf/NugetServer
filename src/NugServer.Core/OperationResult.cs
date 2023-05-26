using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugServer
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    [DebuggerDisplay($"Success={{{nameof(Success)}}}, Message={{{nameof(Message)}}}")]
    public class OperationResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a message associated with the operation result.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        public OperationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class with the specified success value.
        /// </summary>
        /// <param name="success">A value indicating whether the operation was successful.</param>
        public OperationResult(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class with the specified success value and message.
        /// </summary>
        /// <param name="success">A value indicating whether the operation was successful.</param>
        /// <param name="message">A message associated with the operation result.</param>
        public OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
