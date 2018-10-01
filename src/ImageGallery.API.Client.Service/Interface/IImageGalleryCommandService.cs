using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.API.Client.Service.Models;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageGalleryCommandService
    {
        Task PostImageGalleryApi(TokenResponse token, ImageForCreation image, CancellationToken cancellation);

        Task PostImageGalleryApi(TokenResponse token, ImageForCreation image, bool waitForPostComplete, CancellationToken cancellation);
    }
}
