using ImageGallery.API.Client.Test.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.API.Client.Test.Flickr
{
    public class SearchServiceTest : IClassFixture<FlickrFixture>
    {
        private readonly FlickrNet.Flickr _flickr;

        private readonly ITestOutputHelper _output;
        public SearchServiceTest(FlickrFixture fixture, ITestOutputHelper output)
        {
            this._flickr = fixture.Flickr;
            this._output = output;
        }

        [Theory]
        [InlineData("47222519@N07")]
        public async void Can_Get_Photos_By_UserId(string userId)
        {
            var results = await _flickr.PhotosetsGetListAsync(userId, 0, 10);

            Assert.NotEmpty(results);
        }




    }
}
