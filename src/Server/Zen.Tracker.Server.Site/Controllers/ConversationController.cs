using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Bot.Connector.DirectLine.Models;
using Microsoft.Rest;
using Zen.Tracker.Server.Site.Services;

namespace Zen.Tracker.Server.Site.Controllers
{
    /// <summary>
    /// <c>Conversation</c> endpoint is used to retrieve the conversation id
    /// and access token necessary to interact with the tracker bot.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [Authorize]
    [Route("api/conversation")]
    public class ConversationController : ApiController
    {
        private readonly IUserConversationStore _userConversationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationController"/> class.
        /// </summary>
        /// <param name="userConversationStore">The user conversation store.</param>
        public ConversationController(IUserConversationStore userConversationStore)
        {
            _userConversationStore = userConversationStore;
        }

        /// <summary>
        /// Gets the bot conversation for the currently logged on user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Conversation"/> representing both the conversation identifier
        /// and an appropriate access token for the client to use when interacting with it
        /// </returns>
        /// <response code="200">OK</response>
        [HttpGet]
        [ResponseType(typeof(Conversation))]
        public async Task<HttpResponseMessage> GetConversation(CancellationToken cancellationToken)
        {
            var directlineSecret = ConfigurationManager.AppSettings["MicrosoftAppPassword"];
            using (var client = new DirectLineClient(directlineSecret))
            {
                // Use caller's identity (sub) to locate the bot conversation
                //  associated with this user.
                var user = User as ClaimsPrincipal;
                var userId = user?.Claims?.FirstOrDefault(c => c.Type == "sub")?.Value;
                var conversationId = string.IsNullOrEmpty(userId)
                    ? string.Empty
                    : await _userConversationStore
                        .GetConversationByUserIdAsync(userId, cancellationToken)
                        .ConfigureAwait(true);

                // If a conversation exists then attempt to reconnect to it
                if (!string.IsNullOrEmpty(conversationId))
                {
                    using (var response = await client.Tokens
                        .RenewTokenWithHttpMessagesAsync(conversationId, null, cancellationToken)
                        .ConfigureAwait(true))
                    {
                        var conversation = response.HandleError<Conversation>();

                        // Link conversation with caller
                        if (conversation.ConversationId != conversationId)
                        {
                            await _userConversationStore
                                .SetConversationIdAsync(userId, conversation.ConversationId, cancellationToken)
                                .ConfigureAwait(true);
                        }

                        // Return to caller
                        return Request.CreateResponse(HttpStatusCode.OK, conversation);
                    }
                }

                // If a conversation does not exist then create a new one
                using (var response = await client.Tokens
                    .GenerateTokenForNewConversationWithHttpMessagesAsync(null, cancellationToken)
                    .ConfigureAwait(true))
                {
                    var conversation = response.HandleError<Conversation>();

                    // Link conversation with caller
                    await _userConversationStore
                        .SetConversationIdAsync(userId, conversation.ConversationId, cancellationToken)
                        .ConfigureAwait(true);

                    // Return to caller
                    return Request.CreateResponse(HttpStatusCode.OK, conversation);
                }
            }
        }
    }
}
