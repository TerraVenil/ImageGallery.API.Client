using System;
using System.Collections.Generic;
using System.IO;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImageGallery.API.Client.Service.Models;

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

        public static int Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task<int> MainAsync()
        {
            IConfiguration configuration = Configuration;

            var auth = (configuration["openIdConnectConfiguration:authority"]);
            var apisecret = (configuration["openIdConnectConfiguration:apisecret"]);
            var clientId = (configuration["openIdConnectConfiguration:clientId"]);

            var imageGalleryApi = (configuration["imagegallery-api:uri"]);
            var login = (configuration["imagegallery-api:login"]);
            var password = (configuration["imagegallery-api:password"]);

            System.Console.WriteLine($"Auth:{auth}");
            System.Console.WriteLine($"ClientId:{clientId}");

            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync(auth);
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return 1;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, apisecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(login, password, "imagegalleryapi");

            if (tokenResponse.IsError)
            {
                System.Console.WriteLine(tokenResponse.Error);
                return 1;
            }


            System.Console.WriteLine(tokenResponse.Json);
            System.Console.WriteLine("\n\n");
            tokenResponse.Show();
              


            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

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
    }
}

