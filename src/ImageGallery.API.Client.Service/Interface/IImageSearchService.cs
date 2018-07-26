using System.Collections.Generic;
using System.Threading.Tasks;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageSearchService
    {
        Task<IEnumerable<ImageForCreation>>  GetImagesAsync();
    }
}
