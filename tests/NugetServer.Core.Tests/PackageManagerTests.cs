using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NugetServer.Tests
{
    public class PackageManagerTests : IDisposable
    {
        private List<string> _tempFiles = new List<string>();

        private static NugetServerOptions Options = new NugetServerOptions
        {
            PackagesPath = Path.Combine(AppContext.BaseDirectory, "data", "packages")
        };

        private static PackageManager PackageManager = new PackageManager(Options);

        [Fact]
        public void PublishPackage()
        {
            // Arrange
            var packageId = RandomPackageName();
            var version = "1.0.0";

            var pkgInfo = CreatePackage(packageId, version);

            // Act
            PackageManager.PublishPackage(pkgInfo.Path);

            // Assert
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, version, pkgInfo.FullFileName)));
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, version, packageId + ".nuspec")));
        }

        [Fact]
        public void PublishPackage_DeleteAllPreviousVersions()
        {
            // Arrange
            var packageId = RandomPackageName();
            var versions = new[] { "1.0.0", "2.0.0", "3.0.0" };
            var latestVersion = "4.0.0";

            Options.DeletePreviousVersionsOnPublish = false;

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            var latestPackage = CreatePackage(packageId, latestVersion);

            // Act
            Options.DeletePreviousVersionsOnPublish = true;
            PackageManager.PublishPackage(latestPackage.Path);


            // Assert
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, latestVersion, latestPackage.FullFileName)));
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, latestVersion, packageId + ".nuspec")));

            // Assert previous versions are deleted
            Assert.Single(Directory.GetDirectories(Path.Combine(Options.PackagesPath, packageId)));
        }

        [Fact]
        public void PublishPackage_DeletePreviousMinorVersions()
        {
            // Arrange
            var packageId = RandomPackageName();
            var versions = new[] { "1.0.1", "1.2.0", "1.8.2", "2.0.0", "2.1.0" };
            var latestVersion = "2.9.0";

            Options.DeletePreviousMinorVersionsOnPublish = false;

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            var latestPackage = CreatePackage(packageId, latestVersion);

            // Act
            Options.DeletePreviousMinorVersionsOnPublish = true;
            PackageManager.PublishPackage(latestPackage.Path);

            // Assert
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, latestVersion, latestPackage.FullFileName)));
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, latestVersion, packageId + ".nuspec")));

            // Assert previous versions are deleted
            Assert.True(Directory.GetDirectories(Path.Combine(Options.PackagesPath, packageId)).Count() == 5);
        }

        [Fact]
        public void PublishPackage_DeletePreviousPreviewVersions()
        {
            // Arrange
            var packageId = RandomPackageName();
            var versions = new[] { "1.0.1", "1.2.0", "2.0.0-pre", "2.0.0", "2.1.0-pre" };
            var latestVersion = "2.1.0";

            Options.DeletePreviousPreviewVersionsOnPublish = false;

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            var latestPackage = CreatePackage(packageId, latestVersion);

            // Act
            Options.DeletePreviousPreviewVersionsOnPublish = true;
            PackageManager.PublishPackage(latestPackage.Path);

            // Assert
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, latestVersion, latestPackage.FullFileName)));
            Assert.True(File.Exists(Path.Combine(Options.PackagesPath, packageId, latestVersion, packageId + ".nuspec")));

            // Assert previous versions are deleted
            Assert.True(Directory.GetDirectories(Path.Combine(Options.PackagesPath, packageId)).Count() == 5);
        }

        [Fact]
        public void DeletePackage()
        {
            // Arrange
            var packageId = RandomPackageName();
            var versions = new[] { "1.0.0", "2.0.0", "3.0.0" };
            var packageToDeleteVersion = "2.0.0";

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            // Act
            PackageManager.DeletePackage(packageId, packageToDeleteVersion);

            // Assert
            Assert.False(File.Exists(Path.Combine(Options.PackagesPath, packageId, packageToDeleteVersion)));
            Assert.True(Directory.GetDirectories(Path.Combine(Options.PackagesPath, packageId)).Count() == 2);
        }

        [Fact]
        public void GetPackageVersions()
        {
            // Arrange
            var packageId = RandomPackageName();
            var versions = new[] { "1.0.0", "2.0.0", "3.0.0" };

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            // Act
            var packageVersions = PackageManager.GetPackageVersions(packageId);

            // Assert
            Assert.Equal(3, packageVersions.Versions.Length);
        }

        [Fact]
        public void GetPackage()
        {
            // Arrange
            var packageId = RandomPackageName();
            var version = "1.0.0";
            var package = CreatePackage(packageId, version);

            PackageManager.PublishPackage(package.Path);

            // Act
            var pkgContent = PackageManager.GetPackage(packageId, version);

            // Assert
            Assert.Equal(File.ReadAllBytes(package.Path), pkgContent);
        }

        [Fact]
        public void Registration()
        {
            // Arrange
            var packageId = RandomPackageName();
            var versions = new[] { "1.0.0", "2.0.0", "3.0.0" };

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            // Act
            var registration = PackageManager.Registration(packageId);

            // Assert
            Assert.NotNull(registration);
            Assert.True(registration.Items[0].Items.Count == 3);
            Assert.Equal("1.0.0", registration.Items[0].Lower);
            Assert.Equal("3.0.0", registration.Items[0].Upper);
        }

        [Fact]
        public void Query()
        {
            // Arrange
            var keyword = "test";
            var packageId = RandomPackageName() + keyword;
            var versions = new[] { "1.0.0", "2.0.0", "3.0.0" };

            foreach (var version in versions)
            {
                var pkgInfo = CreatePackage(packageId, version);
                PackageManager.PublishPackage(pkgInfo.Path);
            }

            // Act
            var queryResult = PackageManager.Query(keyword);

            // Assert
            Assert.NotNull(queryResult);
        }

        private (string Path, string FullFileName) CreatePackage(string packageId, string version)
        {
            var fullFileName = $"{packageId}.{version}.nupkg";

            var tempPkgPath = Path.Combine(Path.GetTempPath(), $"{packageId}.{version}-{Guid.NewGuid().ToString().Replace("-", "")}.nupkg");

            var tempFile = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll").First();

            var packageBuilder = new PackageBuilder
            {
                Id = packageId,
                Version = new NuGetVersion(version)
            };

            packageBuilder.Authors.Add($"{packageId} Company");
            packageBuilder.Description = "This is a test package";
            packageBuilder.ReleaseNotes = "This is a test package";
            packageBuilder.Summary = "This is a test package";
            packageBuilder.Title = "This is a test package";
            packageBuilder.Tags.Add("test");
            packageBuilder.AddFiles("/", tempFile.FullName, "/lib/net8.0");
            packageBuilder.DependencyGroups.Add(new PackageDependencyGroup(new NuGetFramework("net", new Version("8.0")), Array.Empty<PackageDependency>()));

            using (var fs = new FileStream(tempPkgPath, FileMode.Create))
            {
                packageBuilder.Save(fs);
            }

            _tempFiles.Add(tempPkgPath);

            return (tempPkgPath, fullFileName);
        }

        private string RandomPackageName()
        {
            return $"FakeLib.{Guid.NewGuid().ToString().Replace("-", "")}";
        }

        public void Dispose()
        {
            // delete temp files
            foreach (var item in _tempFiles)
            {
                File.Delete(item);
            }

            foreach (var dir in Directory.GetDirectories(Path.Combine(AppContext.BaseDirectory, "data", "packages")))
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
