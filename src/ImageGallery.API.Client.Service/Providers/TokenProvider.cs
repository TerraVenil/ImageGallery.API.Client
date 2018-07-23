using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.API.Client.Service.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly string _auth;

        private readonly string _apiSecret;

        private readonly string _clientId;

        public string Api => "imagegalleryapi";

        private readonly OpenIdConnectConfiguration _config;

        private readonly ILogger<TokenProvider> _logger;
        public TokenProvider(ILogger<TokenProvider> logger,
            IOptions<OpenIdConnectConfiguration> config)
        {
            _config = config.Value;
            _logger = logger;

            _auth = _config.Authority;
            _apiSecret = _config.ApiSecret;
            _clientId = _config.ClientId;
        }

        public TokenProvider(OpenIdConnectConfiguration configuration)
        {
            _auth = configuration.Authority;
            _apiSecret = configuration.ApiSecret;
            _clientId = configuration.ClientId;
        }

        public async Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password, string api)
        {
            //_logger.LogInformation($"This is a console application for {api}");

            var disco = await DiscoveryClient.GetAsync(_auth);
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return null;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, _clientId, _apiSecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, api);
            if (tokenResponse.IsError)
            {
                System.Console.WriteLine(tokenResponse.Error);
                return null;
            }

            return tokenResponse;
        }

    }
}
