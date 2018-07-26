using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageService : IImageService
    {
        public ImageService()
        {
            
        }

        public ImageModel GetImage(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ImageForCreation> GetImages()
        {
            List<ImageForCreation> imageForCreations = new List<ImageForCreation>();

            string appPath = Directory.GetCurrentDirectory();
            string photoPath = @"../../../../../data/photos";
            var filePath = Path.GetFullPath(Path.Combine(appPath, photoPath));
            var images = new List<string>
            {
                "7444320646_fbc51d1c60_z.jpg",
                "7451503978_ce570a5471_z.jpg",
                "9982986024_0d2a4f9b20_z.jpg",
                "12553248663_e1abd372d1_z.jpg",
                "12845283103_8385e5a19d_z.jpg",
                "25340114767_6ee4be93f6_z.jpg"
            };

            var imageList = images.Select(i => Path.Combine(filePath, i));
            foreach (var fileName in imageList)
            {
                var imageForCreation = new ImageForCreation
                {
                    Category = "Test Category",
                    Title = "Test"
                };

                using (var fileStream = new FileStream(fileName, FileMode.Open))
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();
                }

                 imageForCreations.Add(imageForCreation);
            }

            return imageForCreations;
        }

        public void InsertImage(ImageForCreation value)
        {
            throw new NotImplementedException();
        }

        public void UpdateImage(ImageForUpdate value)
        {
            throw new NotImplementedException();
        }
    }
}
