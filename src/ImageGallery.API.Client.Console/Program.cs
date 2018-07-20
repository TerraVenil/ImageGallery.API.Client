using System;
using System.IO;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading.Tasks;
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
            var apisecret = (configuration["openIdConnectConfiguration:apisecret"]);
            var clientId = (configuration["openIdConnectConfiguration:clientId"]);

            System.Console.WriteLine($"Auth:{auth}");
            System.Console.WriteLine($"ClientId:{clientId}");

            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync(auth);
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, apisecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("Frank", "password", "imagegalleryapi");

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

            var response = await client.GetAsync($"https://imagegallery-api.informationcart.com/api/images");
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

