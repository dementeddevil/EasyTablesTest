using System.Threading;
using System.Threading.Tasks;

namespace Zen.Tracker.Server.Site.Services
{
    /// <summary>
    /// <c>IUserConversationStore</c> defines the linkage between users and conversations.
    /// </summary>
    public interface IUserConversationStore
    {
        /// <summary>
        /// Gets the conversation id for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<string> GetConversationByUserIdAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the conversation identifier for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SetConversationIdAsync(string userId, string conversationId, CancellationToken cancellationToken);
    }
}