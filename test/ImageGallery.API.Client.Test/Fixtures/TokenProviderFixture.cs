using System;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.API.Client.Test.Fixtures
{
    public class TokenProviderFixture : IDisposable
    {
        public TokenProviderFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<ITokenProvider, TokenProvider>()
                .BuildServiceProvider();

            TokenProvider = serviceProvider.GetRequiredService<TokenProvider>();
        }

        public ITokenProvider TokenProvider { get; set; }

        public void Dispose()
        {
        }
    }
}
