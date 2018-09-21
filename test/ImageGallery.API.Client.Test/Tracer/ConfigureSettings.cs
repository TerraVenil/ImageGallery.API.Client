using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ImageGallery.API.Client.Test.Tracer
{
    public class ConfigureSettings
    {
        public static IConfiguration CreateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
