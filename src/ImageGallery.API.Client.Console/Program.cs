using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Polly;
using Polly.Retry;
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

        private static readonly CancellationTokenSource CSource = new CancellationTokenSource();

        public static int RetriesCount = 3;

        public static int Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task<int> MainAsync(string[] args)
        {
            // System.Console.TreatControlCAsInput = true;
            System.Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                System.Console.WriteLine("CANCELLATION REQUEST!");
                CSource.Cancel();
            };

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IConfiguration configuration = Configuration;
            var login = configuration["imagegallery-api:login"];
            var password = configuration["imagegallery-api:password"];
            var api = configuration["imagegallery-api:api"];
            var imageGalleryApi = configuration["imagegallery-api:uri"];


            var isLocalDiskOnly = args?.Contains("/p") ?? false;

            SearchOptions photoSearchOptions = new SearchOptions
            {
                // See for List of Available Machine Tags
                //https://api-attractions.navigatorglass.com/swagger/#!/MachineKey/ApiMachineKeyPredicatesGet

                //MachineTags = "machine_tags => nycparks:",
                //MachineTags = "machine_tags => nychalloffame:",
                MachineTags = "machine_tags => nycparks:m010=",
                //MachineTags = "machine_tags => nycparks:m089=",
                //MachineTags = "machine_tags => nycparks:q436=",
                // MachineTags = "machine_tags => nycparks:m010=114",
                // UserId = "",
                //PhotoSize = "q",   //150x150
                //PhotoSize = "z",   // Medium 640
                PhotoSize = "q",   //width="1024" height="768
            };

            TokenResponse token = null;
            if (!isLocalDiskOnly && !CSource.IsCancellationRequested)
            {
                try
                {
                    Metric.Start("token");
                    token = await TokenProvider.RequestResourceOwnerPasswordAsync(login, password, api);
                    if (token == null)
                    {
                        Log.Error("Token Request Failed");
                        return await Task.FromResult(1);
                    }

                    token.Show();
                }
                finally
                {
                    Metric.StopAndWriteConsole("token");
                }
            }


            try
            {
                Metric.Start("Flickr Search and Post");
                // start processing
                // waitForPostComplete is true by default, waiting when image has finished upload
                //  if we don't need to wait (e.g. no afterward actions are needed) we can set it to false to speed up even more
                PerformGetAndPost(CSource.Token, token, photoSearchOptions, imageGalleryApi, 10, 10, false).ConfigureAwait(false).GetAwaiter().OnCompleted(async () =>
                {
                    System.Console.WriteLine($"Flickr Total API requests: {ImageSearchService.FlickrQueriesCount}");
                    System.Console.WriteLine($"Flickr Total Bytes: {ImageSearchService.FlickrQueriesBytes}");

                    //if (!isLocalDiskOnly && !CSource.IsCancellationRequested)
                    //{
                    //    try
                    //    {
                    //        Metric.Start("get");
                    //        await GetImageGalleryApi(CSource.Token, token, imageGalleryApi);
                    //    }
                    //    finally
                    //    {
                    //        Metric.StopAndWriteConsole("get");
                    //    }
                    //}

                });
            }
            finally
            {
                Metric.StopAndWriteConsole("NEW flickrimg");
            }

            System.Console.ReadLine();

            return 0;
        }

        private static readonly HttpClient HttpClient = new HttpClient();

        public static int ThreadsCount;

        /// <summary>
        /// Uses conveyor queue logic to process images as soon as they are available
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="searchOptions">Flickr Search Options</param>
        /// <param name="apiUri">ImageGallery Api Uri</param>
        /// <param name="threadCount"></param>
        /// <param name="waitForPostComplete"></param>
        /// <returns>
        ///  A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private static async Task<string> PerformGetAndPost(CancellationToken cancellation, TokenResponse token, SearchOptions searchOptions, string apiUri, int threadCount, int postThreadCount, bool waitForPostComplete)
        {
            var limit = System.Net.ServicePointManager.DefaultConnectionLimit;
            System.Net.ServicePointManager.DefaultConnectionLimit = 5;
            // System.Net.ServicePointManager.MaxServicePoints = 1000;
            HttpClient.DefaultRequestHeaders.ConnectionClose = true;
            System.Net.ServicePointManager.ReusePort = true;
            // System.Net.ServicePointManager.SetTcpKeepAlive(false,0,0);

            if (token != null)
                HttpClient.SetBearerToken(token.AccessToken);

            // System.Net.ServicePointManager.MaxServicePoints = threadCount * 3;
            ThreadPool.SetMinThreads(Math.Max(threadCount, postThreadCount) * 2, threadCount);
            int asyncCount = 0;

            var options = new SearchOptions
            {
                PhotoSize = searchOptions.PhotoSize ?? "z",
                MachineTags = searchOptions.MachineTags,
            };

            try
            {
                // start search processing
                ImageSearchService.StartImagesSearchQueue(cancellation, options, threadCount, HttpClient);
                ThreadsCount = 0;

                var policy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        retryCount: RetriesCount, // Retry 3 times
                        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200), // Wait 200ms between each try.
                        onRetry: (exception, calculatedWaitDuration) => // Capture some info for logging!
                        {
                            System.Console.WriteLine($"--> Retrying query due to {exception.InnerException?.Message ?? exception.Message}");
                            Log.Error("{@Status} ImageGalleryAPI Web Query Error! Retrying after {ex}", "ERROR", exception.InnerException?.Message ?? exception.Message);
                        });

                if (token != null) //online
                {
                    // do while image search is running, image queue is not empty or there are some async tasks left
                    while (ImageSearchService.IsSearchRunning || !ImageSearchService.ImageForCreations.IsEmpty || asyncCount > 0)
                    {
                        if (cancellation.IsCancellationRequested)
                            return "ABORTED";
                        // get image from queue
                        if (!ImageSearchService.ImageForCreations.TryDequeue(out var image))
                            continue;

                        // wait for available threads
                        while (asyncCount > postThreadCount) // http threads could stuck if there are too many. had to tweak this param

                        {
                            await Task.Delay(5);
                        }

                        Debug.WriteLine($"ADD MY P: {asyncCount}");
                        asyncCount++;
                        //run new processing thread
                        new Thread(async state =>
                        {
                            if (cancellation.IsCancellationRequested)
                                return;
                            try
                            {
                                await PostImageGalleryApi(HttpClient, policy, image, apiUri, waitForPostComplete, cancellation).ContinueWith(
                                    status =>
                                {
                                    if (!status.Result.IsSuccessStatusCode)
                                        Log.Error("{@Status} ImageGalleryAPI Post Error {@Image}", status.Result.StatusCode.ToString(), image.ToString());
                                    else
                                        Log.Information("{@Status} ImageGalleryAPI Post Complete {@Image}", status.Result.StatusCode, image.ToString());
                                }, cancellation);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("{@Status} ImageGalleryAPI Post (WEB)Error {@Image}", "FAIL", image.ToString());
                            }
                            finally
                            {
                                asyncCount--;
                                Debug.WriteLine($"P: {asyncCount}");
                            }
                        }).Start();
                    }
                }
                else
                {
                    var cfg = Configuration.GetSection("generalConfiguration").Get<GeneralConfiguration>();
                    if (cfg == null)
                        return "Fail - not configured!";
                    //local disk
                    while (ImageSearchService.IsSearchRunning || !ImageSearchService.ImageForCreations.IsEmpty || asyncCount > 0)
                    {
                        if (cancellation.IsCancellationRequested)
                            return "ABORTED";
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
                            if (cancellation.IsCancellationRequested)
                                return;
                            try
                            {
                                if (!ImageHelper.SaveImageFile(cfg.LocalImagesPath, image))
                                {
                                    Log.Error("{@Status} Local Image Save Error {@Image}", "FAIL", image.ToString());
                                }
                                else
                                    Log.Information("{@Status} Local Image Save COMPLETE {@Image}", "OK", image.ToString());
                            }
                            finally
                            {
                                asyncCount--;
                            }
                        });
                        if (cancellation.IsCancellationRequested)
                            return "ABORTED";
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

        /// <summary>
        ///  Image Gallery API - Post Message
        /// </summary>
        /// <param name="client"></param>
        /// <param name="policy"></param>
        /// <param name="image"></param>
        /// <param name="apiUri"></param>
        /// <param name="waitForPostComplete"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> PostImageGalleryApi(HttpClient client, RetryPolicy policy, ImageForCreation image, string apiUri, bool waitForPostComplete,
            CancellationToken cancellation)
        {
            Log.Verbose("ImageGalleryAPI Post {@Image}| {FileSize}", image.ToString(), image.Bytes.Length);

            // TODO - Add Errors to be Handled
            var serializedImageForCreation = JsonConvert.SerializeObject(image);

            var response = await policy.ExecuteAsync(async () => await client.PostAsync(
                    $"{apiUri}/api/images",
                    new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"), cancellation)
                .ConfigureAwait(waitForPostComplete));

            // TODO - Log Transaction Time/Sucess Message
            return response;
        }

        /// <summary>
        /// Image Gallery API - Get Images
        /// </summary>
        /// <param name="cancellation"></param>
        /// <param name="token"></param>
        /// <param name="imageGalleryApi"></param>
        /// <returns></returns>
        private static async Task<string> GetImageGalleryApi(CancellationToken cancellation, TokenResponse token, string imageGalleryApi)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(token.AccessToken);

            var response = await client.GetAsync($"{imageGalleryApi}/api/images", cancellation);
            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var images = JsonConvert.DeserializeObject<List<ImageModel>>(content);
              //  System.Console.WriteLine(JArray.Parse(content));
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
                    .AddLogging()
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
