using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.API.Client.Service.Interface;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Polly;
using Serilog;

namespace ImageGallery.API.Client.Service.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly string _auth;

        private readonly string _apiSecret;

        private readonly string _clientId;

        public string Api => "imagegalleryapi";

        private readonly ApplicationOptions _config;

        private readonly ILogger<TokenProvider> _logger;
        public TokenProvider(IOptions<ApplicationOptions> config, ILogger<TokenProvider> logger)
        {
            _config = config.Value;
            _logger = logger;

            _auth = _config.OpenIdConnectConfiguration.Authority ?? throw new ArgumentNullException(nameof(OpenIdConnectConfiguration.Authority)); ;
            _apiSecret = _config.OpenIdConnectConfiguration.ApiSecret ?? throw new ArgumentNullException(nameof(OpenIdConnectConfiguration.ApiSecret));
            _clientId = _config.OpenIdConnectConfiguration.ClientId ?? throw new ArgumentNullException(nameof(OpenIdConnectConfiguration.ClientId));
        }

        public TokenProvider(OpenIdConnectConfiguration configuration)
        {
            _auth = configuration.Authority ?? throw new ArgumentNullException(nameof(OpenIdConnectConfiguration.Authority));
            _apiSecret = configuration.ApiSecret ?? throw new ArgumentNullException(nameof(OpenIdConnectConfiguration.ApiSecret));
            _clientId = configuration.ClientId ?? throw new ArgumentNullException(nameof(OpenIdConnectConfiguration.ClientId));
        }

        public async Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password, string api)
        {
            try
            {
                //_logger.LogInformation($"This is a console application for {api}");
                var cfg = ConfigurationHelper.GetGeneralConfig();
                var policy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        retryCount: cfg.QueryRetriesCount,
                        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(cfg.QueryWaitBetweenQueries), // Wait 200ms between each try.
                        onRetry: (exception, calculatedWaitDuration) => // Capture some info for logging!
                        {
                            Log.Error("{@Status} ImageGalleryAPI TOKEN Query Error! Exception: {ex}", "ERROR", exception.InnerException?.Message ?? exception.Message);
                        });

                var disco = await policy.ExecuteAsync(async () =>
                {
                    var result = await DiscoveryClient.GetAsync(_auth);
                    if (result.IsError)
                        throw new HttpRequestException(result.Error);
                    return result;
                });
                if (disco.IsError)
                {
                    System.Console.WriteLine(disco.Error);
                    return null;
                }



                var tokenClient = new TokenClient(disco.TokenEndpoint, _clientId, _apiSecret);
                var tokenResponse = await policy.ExecuteAsync(async () => await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, api));
                if (tokenResponse.IsError)
                {
                    System.Console.WriteLine(tokenResponse.Error);
                    return null;
                }

                return tokenResponse;
            }
            catch (Exception ex)
            {
                Log.Error("{@Status} ImageGalleryAPI TOKEN Fatal Error: {ex}", "ERROR", ex.InnerException?.Message ?? ex.Message);
                return null;
            }
        }

    }
}
