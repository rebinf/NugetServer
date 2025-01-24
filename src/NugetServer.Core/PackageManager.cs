using NugetServer.Extensions;
using NugetServer.Models;
using NugetServer.Utilities;
using System;

namespace NugetServer
{
    public class PackageManager
    {
        public NugetServerOptions Options { get; }

        public PackageManager(NugetServerOptions options)
        {
            Options = options;
        }

        public PackageVersions GetPackageVersions(string packageId)
        {
            var packageDirectory = Path.Combine(AppContext.BaseDirectory, Options.PackagesPath, packageId);

            // does package directory exist?
            if (!Directory.Exists(packageDirectory))
            {
                return null;
            }

            // get all versions
            var versions = new DirectoryInfo(packageDirectory).GetDirectories()
                .Select(x => x.Name)
                .ToArray();

            return new PackageVersions() { Versions = versions };
        }

        public byte[] GetPackage(string packageId, string version)
        {
            var packageDirectory = Path.Combine(AppContext.BaseDirectory, Options.PackagesPath, packageId);

            var fileName = Path.Combine(packageDirectory, version, $"{packageId}.{version}.nupkg");

            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }

            return null;
        }

        public OperationResult PublishPackage(string packageFile, bool deleteSourcePackage = false)
        {
            try
            {
                var stream = File.OpenRead(packageFile);

                return PublishPackage(stream);
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
            finally
            {
                if (deleteSourcePackage)
                {
                    File.Delete(packageFile);
                }
            }
        }

        public OperationResult PublishPackage(Stream stream)
        {
            try
            {
                var nupkg = new NuGet.Packaging.PackageArchiveReader(stream);

                // get package id
                var id = nupkg.NuspecReader.GetId().ToLower();

                // package directory
                var packageDirectory = Path.Combine(AppContext.BaseDirectory, Options.PackagesPath, id);

                // does package directory exist?
                if (!Directory.Exists(packageDirectory))
                {
                    Directory.CreateDirectory(packageDirectory);
                }

                // package version
                var version = nupkg.NuspecReader.GetVersion().ToNormalizedString();

                // process previous package versions
                if (Options.PreviousPackagesNeedProcessing())
                {
                    ProcessPreviousPackageVersions(packageDirectory, version);
                }

                // version directory
                var versionDirectory = Path.Combine(packageDirectory, version);

                // does version directory exist?
                if (!Directory.Exists(versionDirectory))
                {
                    Directory.CreateDirectory(versionDirectory);
                }

                // copy nupkg to version directory
                var versionPackageFile = Path.Combine(versionDirectory, $"{id}.{version}.nupkg");

                //delete existing package
                if (File.Exists(versionPackageFile))
                {
                    File.Delete(versionPackageFile);
                }

                // copy nuspec to version directory
                var nuspecFile = Path.Combine(versionDirectory, nupkg.GetNuspecFile());

                using (var nuspecStream = nupkg.GetNuspec())
                {
                    using (var fileStream = new FileStream(nuspecFile, FileMode.Create))
                    {
                        nuspecStream.CopyTo(fileStream);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);

                File.WriteAllBytes(versionPackageFile, stream.ToByteArray());

                stream.Close();

                return new OperationResult(true);
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
        }

        public void DeletePackage(string packageId, string version)
        {
            var packageDirectory = Path.Combine(AppContext.BaseDirectory, Options.PackagesPath, packageId);

            if (!Directory.Exists(packageDirectory))
            {
                return;
            }

            var versionDirectory = Path.Combine(packageDirectory, version);

            if (Directory.Exists(versionDirectory))
            {
                Directory.Delete(versionDirectory, true);
            }
        }

        public RegistrationResult Registration(string packageId)
        {
            var packageDirectory = Path.Combine(AppContext.BaseDirectory, Options.PackagesPath, packageId);

            if (!Directory.Exists(packageDirectory))
            {
                return new RegistrationResult() { Count = 0 };
            }

            var packageDirInfo = new DirectoryInfo(packageDirectory);

            var versions = packageDirInfo.GetDirectories()
                .Select(x => x.Name)
                .ToArray();

            var result = new RegistrationResult()
            {
                Count = versions.Length,
            };

            var url = GetPackageRegistrationUrl(packageId);

            var registration = new Registration
            {
                Id = url,
                Count = 1,
                Lower = GetLowerVersion(packageDirInfo),
                Upper = GetUpperVersion(packageDirInfo),
            };

            result.Items.Add(registration);

            foreach (var v in versions)
            {
                var leaf = new RegistrationLeaf
                {
                    Id = GetPackageRegistrationUrl(packageId, v),
                    PackageContent = UrlBuilder.Combine(Options.BaseUrl, RouteIds.PackageBaseAddress, packageId.ToLower(), v.ToLower(), packageId.ToLower(), $"{v.ToLower()}.nupkg"),
                    CatalogEntry = new CatalogEntry()
                    {
                        Id = GetPackageRegistrationUrl(packageId, v),
                        PackageId = packageId,
                        Version = v,
                    }
                };

                registration.Items.Add(leaf);
            }

            return result;
        }

        public QueryResult Query(string query)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, Options.PackagesPath);

            var packages = string.IsNullOrWhiteSpace(query) ? new DirectoryInfo(dir).GetDirectories().ToArray()
                : new DirectoryInfo(dir).GetDirectories().Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();

            var result = new QueryResult() { TotalHits = packages.Length };

            foreach (var pkgDir in packages)
            {
                var meta = new PackageMetadata
                {
                    Id = GetPackageRegistrationUrl(pkgDir.Name),
                    PackageId = pkgDir.Name,
                    Versions = pkgDir.GetDirectories().Select(x => new PackageVersion() { Id = GetPackageRegistrationUrl(pkgDir.Name, x.Name), Version = x.Name }).ToList(),
                    Version = GetUpperVersion(pkgDir)
                };

                result.Data.Add(meta);
            }

            return result;
        }

        private void ProcessPreviousPackageVersions(string packageDirectory, string version)
        {
            var currentVersion = NuGet.Versioning.NuGetVersion.Parse(version);

            // get all versions
            var allVersions = Directory.GetDirectories(packageDirectory)
                .Select(x => NuGet.Versioning.NuGetVersion.Parse(new DirectoryInfo(x).Name))
                .ToArray();

            // get previous versions
            var previousVersions = allVersions.Where(x => x < currentVersion).ToArray();

            // delete all previous versions
            if (Options.DeletePreviousVersionsOnPublish)
            {
                foreach (var previousVersion in previousVersions)
                {
                    var previousVersionDirectory = Path.Combine(packageDirectory, previousVersion.ToNormalizedString());
                    Directory.Delete(previousVersionDirectory, true);
                }
            }

            // delete previous minor versions
            if (Options.DeletePreviousMinorVersionsOnPublish)
            {
                var previousMinorVersions = previousVersions.Where(x => x.Major == currentVersion.Major && x.Minor > 0 && (x.Minor < currentVersion.Minor || x.Patch < currentVersion.Patch)).ToArray();

                foreach (var previousMinorVersion in previousMinorVersions)
                {
                    var previousMinorVersionDirectory = Path.Combine(packageDirectory, previousMinorVersion.ToNormalizedString());
                    Directory.Delete(previousMinorVersionDirectory, true);
                }
            }

            // delete previous preview versions
            if (Options.DeletePreviousPreviewVersionsOnPublish)
            {
                var previousPreviewVersions = previousVersions.Where(x => x.Major == currentVersion.Major && x.Minor == currentVersion.Minor && (x.Patch == currentVersion.Patch || x.Patch < currentVersion.Patch) && x.IsPrerelease).ToArray();

                foreach (var previousPreviewVersion in previousPreviewVersions)
                {
                    var previousPreviewVersionDirectory = Path.Combine(packageDirectory, previousPreviewVersion.ToNormalizedString());
                    Directory.Delete(previousPreviewVersionDirectory, true);
                }
            }
        }

        private string GetUpperVersion(DirectoryInfo directory)
        {
            return directory.GetDirectories()
                     .Select(x => NuGet.Versioning.NuGetVersion.Parse(x.Name))
                     .OrderByDescending(x => x)
                     .Select(x => x.ToNormalizedString())
                     .FirstOrDefault();
        }

        private string GetLowerVersion(DirectoryInfo directory)
        {
            return directory.GetDirectories()
                     .Select(x => NuGet.Versioning.NuGetVersion.Parse(x.Name))
                     .OrderBy(x => x)
                     .Select(x => x.ToNormalizedString())
                     .FirstOrDefault();
        }

        private string GetPackageRegistrationUrl(string packageId)
        {
            return UrlBuilder.Combine(Options.BaseUrl, RouteIds.RegistrationsBaseUrl, packageId.ToLower(), "index.json");
        }

        private string GetPackageRegistrationUrl(string packageId, string version)
        {
            return UrlBuilder.Combine(Options.BaseUrl, RouteIds.RegistrationsBaseUrl, packageId.ToLower(), version.ToLower(), "index.json");
        }
    }
}
