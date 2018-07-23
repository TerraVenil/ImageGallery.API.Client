using System.Threading.Tasks;
using IdentityModel.Client;

namespace ImageGallery.API.Client.Service.Interface
{
    public interface ITokenProvider
    {
        Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password, string api);
    }
}
