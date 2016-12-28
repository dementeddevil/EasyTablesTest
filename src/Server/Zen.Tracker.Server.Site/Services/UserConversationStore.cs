using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Zen.Tracker.Server.Storage.Entities;
using Zen.Tracker.Server.Storage.Models;

namespace Zen.Tracker.Server.Site.Services
{
    public class UserConversationStore : IUserConversationStore
    {
        public UserConversationStore()
        {
        }

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