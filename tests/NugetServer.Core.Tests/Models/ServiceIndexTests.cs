using NugetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NugetServer.Tests.Models
{
    public class ServiceIndexTests
    {
        [Fact]
        public void ServiceIndexJson()
        {
            // Arrange
            var url = "localhost";

            // Act
            var serviceIndex = ServiceIndex.GetFullServiceIndex(url);
            var json = System.Text.Json.JsonSerializer.Serialize(serviceIndex);

            // Assert
            Assert.NotEmpty(json);
        }
    }
}
