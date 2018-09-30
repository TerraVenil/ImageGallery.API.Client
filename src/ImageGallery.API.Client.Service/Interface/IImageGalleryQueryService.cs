using System.Threading.Tasks;
using IdentityModel.Client;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface IImageGalleryQueryService
    {
        Task<string> GetUserImageCollectionAsync(TokenResponse token);
    }
}
