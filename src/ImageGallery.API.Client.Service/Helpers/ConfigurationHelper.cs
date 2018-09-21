using System;
using System.IO;
using ImageGallery.API.Client.Service.Configuration;
using Microsoft.Extensions.Configuration;

namespace ImageGallery.API.Client.Service.Helpers
{
    public static class ConfigurationHelper
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static GeneralConfiguration GetGeneralConfig()
        {
            return Configuration.GetSection("generalConfiguration").Get<GeneralConfiguration>();
        }

        public static OpenIdConnectConfiguration GetOpenIdConfig()
        {
            return Configuration.GetSection("openIdConnectConfiguration").Get<OpenIdConnectConfiguration>();
        }

        public static FlickrConfiguration GetFlickrConfig()
        {
            return Configuration.GetSection("flickrConfiguration").Get<FlickrConfiguration>();
        }
    }
}
