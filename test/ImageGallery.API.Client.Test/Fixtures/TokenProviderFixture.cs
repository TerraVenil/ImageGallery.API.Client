using System;
using System.IO;
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
                .AddEnvironmentVariables()
                .Build();


            var serviceProvider = new ServiceCollection()
                .AddScoped<ITokenProvider, TokenProvider>()
                .BuildServiceProvider();

            TokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
        }

        public ITokenProvider TokenProvider { get; set; }

        public void Dispose()
        {
        }
    }
}
