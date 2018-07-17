using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageGallery.FlickrService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.API.Client.Test.Fixtures
{
    public class FlickrFixture : IDisposable
    {
        public string ApiKey { get; set; }

        public string Secret { get; set; }

        public FlickrNet.Flickr Flickr { get; set; }

        public FlickrFixture()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Console.WriteLine("ENV:" + env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            ApiKey = builder.GetSection("flickrSettings:apiKey").Value;
            Secret = builder.GetSection("flickrSettings:secret").Value;

            this.Flickr = new FlickrNet.Flickr(ApiKey, Secret);

            var serviceProvider = new ServiceCollection()
                .AddScoped<ISearchService>(s => new SearchService(ApiKey, Secret))
                .BuildServiceProvider();

            SearchService = serviceProvider.GetRequiredService<ISearchService>();

        }

        public ISearchService SearchService { get; private set; }

        public void Dispose()
        {
        }
    }
}
