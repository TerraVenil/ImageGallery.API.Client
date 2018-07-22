using System.Drawing;
using System.IO;

namespace ImageGallery.API.Client.Service.Helpers
{
    public static class ImageHelper
    {

        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;

            using (FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
            }

            return imageData;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public static Image ByteArrayToImage(byte[] source)
        {
            MemoryStream ms = new MemoryStream(source);
            Image ret = Image.FromStream(ms);
            return ret;
        }

    }
}
