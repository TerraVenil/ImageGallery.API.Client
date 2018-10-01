using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageGalleryQueryService
    {
        Task<List<ImageModel>> GetUserImageCollectionAsync(TokenResponse token);

        Task<List<ImageModel>> GetUserImageCollectionAsync(TokenResponse token, CancellationToken cancellation);
    }
}
