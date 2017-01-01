using System.Threading.Tasks;

namespace Zen.Tracker.Client.Services
{
    public interface IAuthenticate
    {
        /// <summary>
        /// Initiates login flow, optionally showing appropriate user-interface
        /// </summary>
        /// <param name="showLoginUI">if set to <c>true</c> [show login UI].</param>
        /// <returns></returns>
        Task<bool> LoginAsync(bool showLoginUI);

        /// <summary>
        /// Initiates logout flow
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync();
    }
}
