using System;
using System.Collections.Generic;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageService
    {
        ImageModel GetImage(string id);

        IEnumerable<ImageForCreation>  GetImages();

        void InsertImage(ImageForCreation value);

        void UpdateImage(ImageForUpdate value);
    }
}
