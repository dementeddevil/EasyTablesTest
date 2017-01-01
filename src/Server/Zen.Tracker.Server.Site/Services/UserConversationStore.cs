using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Zen.Tracker.Server.Storage.Entities;
using Zen.Tracker.Server.Storage.Models;

namespace Zen.Tracker.Server.Site.Services
{
    /// <summary>
    /// <c>UserConversationStore</c> implements the user-to-conversation store interface.
    /// </summary>
    /// <seealso cref="Zen.Tracker.Server.Site.Services.IUserConversationStore" />
    public class UserConversationStore : IUserConversationStore
    {
        /// <summary>
        /// Gets the conversation id for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<string> GetConversationByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var context = new MobileServiceContext())
            {
                var link = await context.UserConversations
                    .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken)
                    .ConfigureAwait(false);
                return link?.ConversationId;
            }
        }

        /// <summary>
        /// Sets the conversation identifier for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="conversationId">The conversation identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task SetConversationIdAsync(string userId, string conversationId, CancellationToken cancellationToken)
        {
            using (var context = new MobileServiceContext())
            {
                var link = await context.UserConversations
                    .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken)
                    .ConfigureAwait(false);
                if (link == null)
                {
                    link = new UserConversation
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        ConversationId = conversationId
                    };
                    context.UserConversations.Add(link);
                }
                else if (link.ConversationId == conversationId)
                {
                    return;
                }
                else
                {
                    link.ConversationId = conversationId;
                }
                
                await context
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}