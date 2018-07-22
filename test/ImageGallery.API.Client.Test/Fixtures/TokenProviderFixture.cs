using System;
using System.IO;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.API.Client.Test.Fixtures
{
    public class TokenProviderFixture : IDisposable
    {
        public TokenProviderFixture()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Console.WriteLine("ENV:" + env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            var openIdConfig = config.GetSection("openIdConnectConfiguration").Get<OpenIdConnectConfiguration>();

            var serviceProvider = new ServiceCollection()
                .AddScoped<ITokenProvider>(c => new TokenProvider(openIdConfig))
                .BuildServiceProvider();

            TokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
        }

        public ITokenProvider TokenProvider { get; set; }

        public void Dispose()
        {
        }
    }
}
