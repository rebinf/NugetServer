
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using NugServer;
using NugServer.Controllers;
using NugServer.Models;

namespace NugServer
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
            builder.Services.Configure<NugServerOptions>(builder.Configuration.GetSection(nameof(NugServerOptions)));

            ProcessEnvironmentVariables(builder.Configuration);
            ProcessPassedArguments(args, builder.Configuration);

            Console.WriteLine($"NugServer BaseUrl: {builder.Configuration.GetSection(nameof(NugServerOptions))[nameof(NugServerOptions.BaseUrl)]}");

            builder.Services.AddControllers().AddJsonOptions(cfg => cfg.JsonSerializerOptions.WriteIndented = true);

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }

        // there is probably a better way to do this
        private static void ProcessEnvironmentVariables(ConfigurationManager configurationManager)
        {
            var configuration = configurationManager.GetSection(nameof(NugServerOptions));

            if (Environment.GetEnvironmentVariable("NUG_URL") is string url)
            {
                configuration[nameof(NugServerOptions.BaseUrl)] = url;
            }

            if (Environment.GetEnvironmentVariable("API_KEY") is string apiKey)
            {
                configuration[nameof(NugServerOptions.ApiKey)] = apiKey;
            }

            if (Environment.GetEnvironmentVariable("DEL_ALL") is string delAll)
            {
                configuration[nameof(NugServerOptions.DeletePreviousVersionsOnPublish)] = delAll;
            }

            if (Environment.GetEnvironmentVariable("DEL_MIN") is string delMinor)
            {
                configuration[nameof(NugServerOptions.DeletePreviousMinorVersionsOnPublish)] = delMinor;
            }

            if (Environment.GetEnvironmentVariable("DEL_PRE") is string delPre)
            {
                configuration[nameof(NugServerOptions.DeletePreviousPreviewVersionsOnPublish)] = delPre;
            }
        }

        private static void ProcessPassedArguments(string[] args, ConfigurationManager configurationManager)
        {
            var configuration = configurationManager.GetSection(nameof(NugServerOptions));

            if (args.FirstOrDefault(x => x.StartsWith("--NUG_URL=", StringComparison.OrdinalIgnoreCase)) is string url)
            {
                configuration[nameof(NugServerOptions.BaseUrl)] = url["--NUG_URL=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--API_KEY=", StringComparison.OrdinalIgnoreCase)) is string apiKey)
            {
                configuration[nameof(NugServerOptions.ApiKey)] = apiKey["--API_KEY=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--DEL_ALL=", StringComparison.OrdinalIgnoreCase)) is string delAll)
            {
                configuration[nameof(NugServerOptions.DeletePreviousVersionsOnPublish)] = delAll["--DEL_ALL=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--DEL_MIN=", StringComparison.OrdinalIgnoreCase)) is string delMinor)
            {
                configuration[nameof(NugServerOptions.DeletePreviousMinorVersionsOnPublish)] = delMinor["--DEL_MIN=".Length..];
            }

            if (args.FirstOrDefault(x => x.StartsWith("--DEL_PRE=", StringComparison.OrdinalIgnoreCase)) is string delPre)
            {
                configuration[nameof(NugServerOptions.DeletePreviousPreviewVersionsOnPublish)] = delPre["--DEL_PRE=".Length..];
            }
        }
    }
}