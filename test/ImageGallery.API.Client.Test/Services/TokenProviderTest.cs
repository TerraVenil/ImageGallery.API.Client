using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Test.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.API.Client.Test.Services
{
    public class TokenProviderTest : IClassFixture<TokenProviderFixture>
    {
        private readonly ITokenProvider _tokenProvider;

        private readonly ITestOutputHelper _output;
        public TokenProviderTest(TokenProviderFixture fixture, ITestOutputHelper output)
        {
            this._tokenProvider = fixture.TokenProvider;
            this._output = output;
        }

        [Theory]
        [InlineData("Frank","password", "imagegalleryapi")]
        public async void Can_Get_User_Token(string userName, string password, string api)
        {
            var result = await _tokenProvider.RequestResourceOwnerPasswordAsync(userName, password, api);
            Assert.NotEmpty(result.AccessToken);
        }

    }
}
