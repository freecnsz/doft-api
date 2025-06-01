using System.Threading.Tasks;

namespace doft.Application.Interfaces.ServiceInterfaces
{
    public interface IS3Service
    {
        Task<string> UploadProfilePictureAsync(string userId, Stream imageStream);
        Task<string?> GetProfilePictureUrlAsync(string userId);
        Task DeleteProfilePictureAsync(string userId);
    }
} 