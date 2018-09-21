using System.IO;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;
using ImageGallery.API.Client.Test.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.API.Client.Test.Services
{
    public class ImageSearchServiceTest : IClassFixture<ImageServiceFixture>
    {
        private readonly IImageSearchService _imageSearchService;

        private readonly ITestOutputHelper _output;

        private readonly string _photoDirectory;

        public ImageSearchServiceTest(ImageServiceFixture fixture, ITestOutputHelper output)
        {
            _imageSearchService = fixture.ImageSearchService;
            _photoDirectory = fixture.PhotoDirectory;

            this._output = output;
        }

        [Theory]
        [InlineData("2645bd94-3614-43fc-b21f-1209d730fc71")]
        public void Can_Get_Image(string id)
        {
            //var result = _imageService.GetImage(id);
            Assert.True(false, "TODO");
        }


        [Fact]
        public void Can_Get_Images()
        {
            //var result = _imageService.GetImages();
            Assert.True(false, "TODO");
        }

        [Fact]
        public void Can_Update_Image()
        {
            Assert.True(false, "TODO");
        }


        [Theory]
        [InlineData("7444320646_fbc51d1c60_z.jpg")]
        public void Can_Create_Image(string imageFileName)
        {
            var photoPath = Path.Combine(_photoDirectory, imageFileName);
            var bytes = ImageHelper.ReadImageFile(photoPath);

            var image = new ImageForCreation
            {
                Bytes = bytes,
                Category = "Test",
                Title = "Test"
            };

            //_imageService.InsertImage(image);

            Assert.True(false, "TODO");
        }
    }
}
