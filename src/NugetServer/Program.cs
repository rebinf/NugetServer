
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using NugetServer;
using NugetServer.Controllers;
using NugetServer.Models;

namespace NugetServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load configuration from /data directory if appsettings.json exists in the directory.
            var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "data");
            var packagesDirectory = Path.Combine(dataDirectory, "packages");

            Directory.CreateDirectory(dataDirectory);
            Directory.CreateDirectory(packagesDirectory);

            File.Copy(Path.Combine(builder.Environment.ContentRootPath, "appsettings.json"), Path.Combine(dataDirectory, "appsettings.json"), true);

            builder.Configuration.AddJsonFile(Path.Combine(dataDirectory, "appsettings.json"), optional: true);

            // strongly typed configuration
            builder.Services.Configure<NugetServerOptions>(builder.Configuration.GetSection(nameof(NugetServerOptions)));

            ProcessEnvironmentVariables(builder.Configuration);
            ProcessPassedArguments(args, builder.Configuration);

            Console.WriteLine($"NugetServer BaseUrl: {builder.Configuration.GetSection(nameof(NugetServerOptions))[nameof(NugetServerOptions.BaseUrl)]}");

            builder.Services.AddControllers().AddJsonOptions(cfg => cfg.JsonSerializerOptions.WriteIndented = true);

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }

        // there is probably a better way to do this
        private static void ProcessEnvironmentVariables(ConfigurationManager configurationManager)
        {
            var configuration = configurationManager.GetSection(nameof(NugetServerOptions));

            if (Environment.GetEnvironmentVariable("NUG_URL") is string url)
            {
                configuration[nameof(NugetServerOptions.BaseUrl)] = url;
            }

            if (Environment.GetEnvironmentVariable("REQ_KEY") is string reqKey)
            {
                configuration[nameof(NugetServerOptions.RequireApiKey)] = reqKey;
            }

            if (Environment.GetEnvironmentVariable("API_KEY") is string apiKey)
            {
                configuration[nameof(NugetServerOptions.ApiKey)] = apiKey;
            }

            if (Environment.GetEnvironmentVariable("DEL_ALL") is string delAll)
            {
                configuration[nameof(NugetServerOptions.DeletePreviousVersionsOnPublish)] = delAll;
            }

            if (Environment.GetEnvironmentVariable("DEL_MIN") is string delMinor)
            {
                configuration[nameof(NugetServerOptions.DeletePreviousMinorVersionsOnPublish)] = delMinor;
            }

            if (Environment.GetEnvironmentVariable("DEL_PRE") is string delPre)
            {
                configuration[nameof(NugetServerOptions.DeletePreviousPreviewVersionsOnPublish)] = delPre;
            }
        }

        private static void ProcessPassedArguments(string[] args, ConfigurationManager configurationManager)
        {
            var configuration = configurationManager.GetSection(nameof(NugetServerOptions));

            if (args.FirstOrDefault(x => x.StartsWith("--NUG_URL=", StringComparison.OrdinalIgnoreCase)) is string url)
            {
                configuration[nameof(NugetServerOptions.BaseUrl)] = url["--NUG_URL=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--REQ_KEY=", StringComparison.OrdinalIgnoreCase)) is string reqKey)
            {
                configuration[nameof(NugetServerOptions.RequireApiKey)] = reqKey["--REQ_KEY=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--API_KEY=", StringComparison.OrdinalIgnoreCase)) is string apiKey)
            {
                configuration[nameof(NugetServerOptions.ApiKey)] = apiKey["--API_KEY=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--DEL_ALL=", StringComparison.OrdinalIgnoreCase)) is string delAll)
            {
                configuration[nameof(NugetServerOptions.DeletePreviousVersionsOnPublish)] = delAll["--DEL_ALL=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--DEL_MIN=", StringComparison.OrdinalIgnoreCase)) is string delMinor)
            {
                configuration[nameof(NugetServerOptions.DeletePreviousMinorVersionsOnPublish)] = delMinor["--DEL_MIN=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--DEL_PRE=", StringComparison.OrdinalIgnoreCase)) is string delPre)
            {
                configuration[nameof(NugetServerOptions.DeletePreviousPreviewVersionsOnPublish)] = delPre["--DEL_PRE=".Length..];
            }
        }
    }
}