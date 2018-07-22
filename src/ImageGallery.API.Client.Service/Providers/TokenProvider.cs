using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Interface;
using Microsoft.Extensions.Options;

namespace ImageGallery.API.Client.Service.Providers
{
    public class TokenProvider : ITokenProvider
    {
        public string Auth => "https://auth.informationcart.com";

        public string ApiSecret => "secret";

        public string ClientId => "navigatorphotoagentui-dev";

        public string Api => "imagegalleryapi";

        private ApplicationOptions _settings;

        //public TokenProvider(IOptions<ApplicationOptions> settings)
        //{
        //    this._settings = settings.Value;
        //}

        public TokenProvider()
        {
            
        }

        public async Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password)
        {
            var disco = await DiscoveryClient.GetAsync(Auth);
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return null;
            }


            var tokenClient = new TokenClient(disco.TokenEndpoint, ClientId, ApiSecret);
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
