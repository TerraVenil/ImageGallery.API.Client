using FlickrNet;
using ImageGallery.API.Client.Test.Fixtures;
using ImageGallery.FlickrService;
using ImageGallery.FlickrService.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.API.Client.Test.Flickr
{
    public class SearchServiceTest : IClassFixture<FlickrFixture>
    {
        private readonly FlickrNet.Flickr _flickr;

        private readonly IFlickrSearchService _flickrSearchService;

        private readonly ITestOutputHelper _output;
        public SearchServiceTest(FlickrFixture fixture, ITestOutputHelper output)
        {
            this._flickr = fixture.Flickr;
            this._flickrSearchService = fixture.SearchService;
            this._output = output;
        }

        [Theory]
        [InlineData("9250911801")]
        public async void Can_Get_Photo_Info(string photoId)
        {
            var photoInfo = await _flickrSearchService.GetPhotoInfoAsync(photoId);
            foreach (var tag in photoInfo.Tags)
            {
                _output.WriteLine($"IsMachineTag{tag.IsMachineTag}");
            }

            Assert.NotNull(photoInfo);
        }

        [Theory]
        [InlineData("9250911801", "n")]
        public async void Can_Get_Photo_Url(string photoId, string size)
        {
            var photoInfo = await _flickrSearchService.GetPhotoInfoAsync(photoId);
            var url = photoInfo.GetPhotoUrl(size);

            Assert.NotNull(photoInfo);
            Assert.NotNull(url);
        }


        [Theory]
        [InlineData("machine_tags => nychalloffame:")]
        public async void Can_Search_Photos_by_MachineTag_Namespace(string namespaceQuery)
        {
            var photoSearchOptions = new PhotoSearchOptions()
            {
                MachineTags = namespaceQuery,
                Extras = PhotoSearchExtras.All,

            };

            var photoCollection = await _flickrSearchService.SearchPhotosAsync(photoSearchOptions);
            Assert.NotNull(photoCollection);
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
