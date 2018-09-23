using System.IO;
using ImageGallery.API.Client.Service.Helpers;
using Xunit;

namespace ImageGallery.API.Client.Test.Helper
{
    public class ImageHelperTest
    {
        public ImageHelperTest()
        {
            string appPath = Directory.GetCurrentDirectory();
            string photoPath = @"../../../../../data/photos";

            PhotoDirectory = Path.GetFullPath(Path.Combine(appPath, photoPath));
        }

        public string PhotoDirectory { get; private set; }


        [Theory]
        [InlineData("7444320646_fbc51d1c60_z.jpg")]
        public void Can_Convert_Image_To_Bytes(string image)
        {
            var photoPath = Path.Combine(PhotoDirectory, image);
            var bytes = ImageHelper.ReadImageFile(photoPath);

            Assert.NotEmpty(bytes);
        }

    }
}
