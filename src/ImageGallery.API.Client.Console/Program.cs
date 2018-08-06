using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Console.Classes;
using ImageGallery.API.Client.Service.Classes;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.API.Client.Service.Providers;
using ImageGallery.API.Client.Service.Services;
using ImageGallery.FlickrService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ImageGallery.API.Client.Console
{
    public class Program
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

        public static IImageService ImageService { get; set; }

        public static IImageSearchService ImageSearchService { get; set; }

        public static int Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task<int> MainAsync()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IConfiguration configuration = Configuration;
            var login = configuration["imagegallery-api:login"];
            var password = configuration["imagegallery-api:password"];
            var api = configuration["imagegallery-api:api"];
            var imageGalleryApi = configuration["imagegallery-api:uri"];

            TokenResponse token;
            try
            {
                Metric.Start("token");
                token = await TokenProvider.RequestResourceOwnerPasswordAsync(login, password, api);
                token.Show();
            }
            finally
            {
                Metric.StopAndWriteConsole("token");
            }


            try
            {
                Metric.Start("Flickr Search and Post");
                // start processing
                // waitForPostComplete is true by default, waiting when image has finished upload
                //  if we don't need to wait (e.g. no afterward actions are needed) we can set it to false to speed up even more
                await PerformGetAndPost(token, imageGalleryApi, 30, true);
            }
            finally
            {
                Metric.StopAndWriteConsole("NEW flickrimg");
            }

            try
            {
                Metric.Start("get");
                await GoGet(token, imageGalleryApi);
            }
            finally
            {
                Metric.StopAndWriteConsole("get");
            }

            System.Console.ReadLine();
            return 0;
        }

        /// <summary>
        /// Uses conveyor queue logic to process images as soon as they are available
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="apiUri">ImageGallery Api Uri</param>
        /// <param name="threadCount"></param>
        /// <param name="waitForPostComplete"></param>
        /// <returns>
        ///  A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private static async Task<string> PerformGetAndPost(TokenResponse token, string apiUri, int threadCount, bool waitForPostComplete)
        {
            var limit = System.Net.ServicePointManager.DefaultConnectionLimit;
            System.Net.ServicePointManager.DefaultConnectionLimit = threadCount * 2;
            ThreadPool.SetMinThreads(threadCount * 2, 4);

            var searchOptions = new SearchOptions
            {
                PhotoSize = "z",
                MachineTags = "machine_tags => nycparks:",
            };

            try
            {
                // start search processing
                ImageSearchService.StartImagesSearchQueue(searchOptions, threadCount);

                int asyncCount = 0;

                // use single client for all queries
                using (var client = new HttpClient())
                {
                    client.SetBearerToken(token.AccessToken);

                    // do while image search is running, image queue is not empty or there are some async tasks left
                    while (ImageSearchService.IsSearchRunning || !ImageSearchService.ImageForCreations.IsEmpty || asyncCount > 0)
                    {
                        // get image from queue
                        if (!ImageSearchService.ImageForCreations.TryDequeue(out var image))
                            continue;

                        // wait for available threads
                        while (asyncCount > threadCount) // http threads could stuck if there are too many. had to tweak this param
                        {
                            await Task.Delay(5);
                        }

                        asyncCount++;
                        //run new processing thread
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            try
                            {
                               var status = GoPostImage(client, image, apiUri, waitForPostComplete).GetAwaiter().GetResult();
                               if (!status.IsSuccessStatusCode)
                               {
                                   Log.Error($"{status.StatusCode.ToString()}");
                               }
                            }
                            finally
                            {
                                asyncCount--;
                            }
                        });
                    }
                }

                // Return Status Code
                return "Sucess";
            }
            finally
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = limit;
            }
        }

        private static async Task<string> GoPostList(IEnumerable<ImageForCreation> images, TokenResponse token, string imageGalleryApi, int threadCount, bool waitForPostComplete)
        {
            System.Console.WriteLine($"Posting {images.Count()} images...");
            using (var client = new HttpClient())
            {
                client.SetBearerToken(token.AccessToken);

                await AsyncHelper.RunWithMaxDegreeOfConcurrency(threadCount, images, async image =>
                {
                    System.Console.WriteLine($"Posting {image.ToString()} ...");
                    var serializedImageForCreation = JsonConvert.SerializeObject(image);
                    var response = await client.PostAsync(
                            $"{imageGalleryApi}/api/images",
                            new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"))
                        .ConfigureAwait(waitForPostComplete);
                    System.Console.WriteLine($"{image.ToString()} post complete!");

                });
            }
            return "Sucess";
        }

        private static async Task<HttpResponseMessage> GoPostImage(HttpClient client, ImageForCreation image, string apiUri, bool waitForPostComplete)
        {
            Log.Information("ImageGalleryAPI Post {@Image}", image.ToString());

            // TODO - Add Errors to be Handled
            var serializedImageForCreation = JsonConvert.SerializeObject(image);

            var response = await client.PostAsync(
                    $"{apiUri}/api/images",
                    new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(waitForPostComplete);

            // TODO - Log Transaction Time/Sucess Message
            if (waitForPostComplete)
                Log.Information("{@Status} Post Complete {@Image}", response.StatusCode, image.ToString());

            return response;
        }

        private static async Task<HttpResponseMessage> GoPost(TokenResponse token, string imageGalleryApi)
        {
            // create an ImageForCreation instance
            var imageForCreation = new ImageForCreation()
            {
                Title = "Test Title",
                Category = "Test Category",
            };

            string appPath = Directory.GetCurrentDirectory();
            string photoPath = @"../../../../../data/photos";
            var filePath = Path.GetFullPath(Path.Combine(appPath, photoPath));
            var fileName = Path.Combine(filePath, "9982986024_0d2a4f9b20_z.jpg");

            using (var fileStream = new FileStream(fileName, FileMode.Open))
            using (var ms = new MemoryStream())
            {
                fileStream.CopyTo(ms);
                imageForCreation.Bytes = ms.ToArray();
            }

            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);

            using (var client = new HttpClient())
            {
                client.SetBearerToken(token.AccessToken);
                var response = await client.PostAsync(
                    $"{imageGalleryApi}/api/images",
                        new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"))
                    .ConfigureAwait(false);

                return response;
            }
        }

        private static async Task<string> GoGet(TokenResponse token, string imageGalleryApi)
        {
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
                return content;
            }

            return null;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            try
            {
                var log = new LoggerConfiguration()
                    .WriteTo.ColoredConsole()
                    .CreateLogger();

                Log.Logger = log;
                Log.Information("The global logger has been configured");

                Metric.Start("config");
                serviceCollection.AddSingleton(new LoggerFactory()
                    .AddConsole()
                    .AddDebug());
                serviceCollection.AddLogging();

                serviceCollection.AddOptions();
                serviceCollection.Configure<OpenIdConnectConfiguration>(Configuration.GetSection("openIdConnectConfiguration"));

                var openIdConfig = Configuration.GetSection("openIdConnectConfiguration").Get<OpenIdConnectConfiguration>();
                var flickrConfig = Configuration.GetSection("flickrConfiguration").Get<FlickrConfiguration>();

                var serviceProvider = new ServiceCollection()
                    .AddScoped<ITokenProvider>(_ => new TokenProvider(openIdConfig))
                    .AddScoped<ISearchService>(_ => new SearchService(flickrConfig.ApiKey, flickrConfig.Secret))
                    .AddScoped<IImageService, ImageService>()
                    .AddScoped<IImageSearchService, ImageSearchService>()
                    .BuildServiceProvider();

                TokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
                ImageService = serviceProvider.GetRequiredService<IImageService>();
                ImageSearchService = serviceProvider.GetRequiredService<IImageSearchService>();
            }
            finally
            {
                Metric.StopAndWriteConsole("config");
            }
        }
    }
}
