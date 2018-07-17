using System;
using System.Collections.Generic;
using System.IO;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.API.Client.Console.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

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

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            IConfiguration configuration = Configuration;

            var auth = (configuration["openIdConnectConfiguration:authority"]);
            var clientId = (configuration["openIdConnectConfiguration:clientId"]);

            System.Console.WriteLine($"Auth:{auth}");
            System.Console.WriteLine($"ClientId:{clientId}");






            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("https://proxy-nginx:8081");
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "imagegalleryclient06", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("imagegalleryapi");
             
            if (tokenResponse.IsError)
            {
                System.Console.WriteLine(tokenResponse.Error);
                return;
            }

            System.Console.WriteLine(tokenResponse.Json);
            System.Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://proxy-nginx:8081/identity");
            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}

