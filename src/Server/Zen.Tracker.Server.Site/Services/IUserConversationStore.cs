using System.Threading;
using System.Threading.Tasks;

namespace Zen.Tracker.Server.Site.Services
{
    public interface IUserConversationStore
    {
        Task<string> GetConversationByUserIdAsync(string userId, CancellationToken cancellationToken);

        Task SetConversationIdAsync(string userId, string conversationId, CancellationToken cancellationToken);
    }
}