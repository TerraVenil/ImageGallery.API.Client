using System;
using ImageGallery.API.Client.Test.Fixtures;
using ImageGallery.FlickrService;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.API.Client.Test.Flickr
{
    public class SearchServiceTest : IClassFixture<FlickrFixture>
    {
        private readonly FlickrNet.Flickr _flickr;

        private readonly ISearchService searchService; 

        private readonly ITestOutputHelper _output;
        public SearchServiceTest(FlickrFixture fixture, ITestOutputHelper output)
        {
            this._flickr = fixture.Flickr;
            this.searchService = fixture.SearchService;
            this._output = output;
        }

        [Theory]
        [InlineData("9250911801")]
        public async void Can_Get_Photo_Info(string photoId)
        {
            var photoInfo = await searchService.GetPhotoInfo(photoId);
            foreach (var tag in photoInfo.Tags)
            {
                _output.WriteLine($"IsMachineTag{tag.IsMachineTag}");
            }

            Assert.NotNull(photoInfo);
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
