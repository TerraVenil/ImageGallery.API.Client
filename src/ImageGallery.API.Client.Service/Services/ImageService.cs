using System;
using System.Collections.Generic;
using ImageGallery.API.Client.Service.Interface;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Services
{
    public class ImageService : IImageService
    {
        public ImageModel GetImage(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ImageModel> GetImages()
        {
            throw new NotImplementedException();
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
