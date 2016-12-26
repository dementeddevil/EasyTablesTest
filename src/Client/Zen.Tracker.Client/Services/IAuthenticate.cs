using System.Threading.Tasks;

namespace Zen.Tracker.Client.Services
{
    public interface IAuthenticate
    {
        Task<bool> LoginAsync();

        Task LogoutAsync();
    }
}
