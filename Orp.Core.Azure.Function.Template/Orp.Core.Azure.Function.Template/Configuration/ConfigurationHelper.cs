using System;
using System.Collections.Generic;
using System.Text;

namespace Orp.Core.Azure.Function.Template.Configuration
{
    internal class ConfigurationHelper
    {
        internal string AppBaseDirectory { get; set; }
        internal string ConfigCertThumbprint => Environment.GetEnvironmentVariable("CONFIG_CERT_THUMBPRINT");
        internal string CurrentEnvironment => Environment.GetEnvironmentVariable("ENVIRONMENT");

        public ConfigurationHelper(string appBaseDirectory)
        {
            AppBaseDirectory = string.IsNullOrWhiteSpace(appBaseDirectory)
                ? throw new ArgumentNullException(nameof(appBaseDirectory))
                : appBaseDirectory;
        }
    }
}
