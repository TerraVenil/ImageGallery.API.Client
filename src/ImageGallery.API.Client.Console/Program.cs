using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.API.Client.Service.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.API.Client.Service.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.API.Client.Console
{
    class Program
    {
        /// <summary>
        ///
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        public static ITokenProvider TokenProvider { get; set; }

        public static int Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task<int> MainAsync()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IConfiguration configuration = Configuration;
            var login = (configuration["imagegallery-api:login"]);
            var password = (configuration["imagegallery-api:password"]);
            var api = (configuration["imagegallery-api:api"]);
            var imageGalleryApi = (configuration["imagegallery-api:uri"]);

            var token = await TokenProvider.RequestResourceOwnerPasswordAsync(login, password, api);
            token.Show();


            // call api
            var client = new HttpClient();
            client.SetBearerToken(token.AccessToken);

            var response = await client.GetAsync($"{imageGalleryApi}/api/images");
            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var images = JsonConvert.DeserializeObject<List<ImageModel>>(content);
                System.Console.WriteLine(JArray.Parse(content));
                System.Console.WriteLine($"ImagesCount:{images.Count}");
            }

            System.Console.ReadLine();
            return 0;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();
            serviceCollection.Configure<OpenIdConnectConfiguration>(Configuration.GetSection("openIdConnectConfiguration"));

            var openIdConfig = Configuration.GetSection("openIdConnectConfiguration").Get<OpenIdConnectConfiguration>();

            var serviceProvider = new ServiceCollection()
                .AddScoped<ITokenProvider>(c => new TokenProvider(openIdConfig))
                .BuildServiceProvider();

            TokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
        }
    }
}

