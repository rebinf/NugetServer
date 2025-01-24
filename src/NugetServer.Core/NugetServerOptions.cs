﻿namespace NugetServer
{
    /// <summary>
    /// Represents the options for the NugetServer.
    /// </summary>
    public class NugetServerOptions
    {
        /// <summary>
        /// Gets or sets the base URL for the NugetServer.
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:5078";

        /// <summary>
        /// Gets or sets a value indicating whether to require an API key for publishing or deleting packages.
        /// </summary>
        public bool RequireApiKey { get; set; } = false;

        /// <summary>
        /// Gets or sets the API key for the NugetServer.
        /// </summary>
        public string ApiKey = "1234567890";

        /// <summary>
        /// Gets or sets the path to the packages directory.
        /// </summary>
        public string PackagesPath { get; set; } = "data/packages";

        /// <summary>
        /// Gets or sets a value indicating whether to delete previous versions of the uploaded package on publish.
        /// </summary>
        public bool DeletePreviousVersionsOnPublish { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to delete previous minor versions of the uploaded package on publish.
        /// </summary>
        public bool DeletePreviousMinorVersionsOnPublish { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to delete previous preview versions of the uploaded package on publish.
        /// </summary>
        public bool DeletePreviousPreviewVersionsOnPublish { get; set; } = false;

        /// <summary>
        /// Indicates whether previous packages need processing.
        /// </summary>
        /// <returns></returns>
        public bool PreviousPackagesNeedProcessing()
        {
            return DeletePreviousVersionsOnPublish || DeletePreviousMinorVersionsOnPublish || DeletePreviousPreviewVersionsOnPublish;
        }
    }
}