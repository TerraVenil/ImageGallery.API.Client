using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;

namespace ImageGallery.API.Client.Service.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly string _auth;

        private readonly string _apiSecret;

        private readonly string _clientId;

        public string Api => "imagegalleryapi";

        public TokenProvider(OpenIdConnectConfiguration configuration)
        {
            _auth = configuration.Authority;
            _apiSecret = configuration.ApiSecret;
            _clientId = configuration.ClientId;
        }

        public async Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password)
        {
            var disco = await DiscoveryClient.GetAsync(_auth);
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return null;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, _clientId, _apiSecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, Api);
            if (tokenResponse.IsError)
            {
                System.Console.WriteLine(tokenResponse.Error);
                return null;
            }

            return tokenResponse;
        }

    }
}
