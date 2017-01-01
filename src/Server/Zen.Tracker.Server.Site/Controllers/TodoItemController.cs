using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Microsoft.Azure.Mobile.Server;
using Zen.Tracker.Server.Storage.Entities;
using Zen.Tracker.Server.Storage.Models;

namespace Zen.Tracker.Server.Site.Controllers
{
    /// <summary>
    /// <c>TodoItem</c> endpoint exposes CRUD operations for todo items.
    /// </summary>
    /// <seealso cref="TableController{TodoItem}" />
    [Authorize]
    public class TodoItemController : TableController<TodoItem>
    {
        /// <summary>
        /// Gets all todo items associated with the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IQueryable<TodoItem>))]
        public IQueryable<TodoItem> GetAll()
        {
            var userId = GetCallerUserIdentifier();
            return Query().Where(t => t.UserId == userId);
        }

        /// <summary>
        /// Gets a single todo item associated with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(SingleResult<TodoItem>))]
        public SingleResult<TodoItem> GetById(string id)
        {
            var item = Lookup(id);
            if (item.Queryable.FirstOrDefault()?.UserId != GetCallerUserIdentifier())
            {
                return new SingleResult<TodoItem>(new TodoItem[0].AsQueryable());
            }

            return item;
        }

        /// <summary>
        /// Patches the todo item associated with the specified identifier
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patch">The patch.</param>
        /// <returns></returns>
        [HttpPatch]
        [ResponseType(typeof(TodoItem))]
        public async Task<TodoItem> Patch(string id, Delta<TodoItem> patch)
        {
            if (!await ValidateItemIdIsAssociatedWithCallerUserAsync(id).ConfigureAwait(true))
            {
                return null;
            }

            return await UpdateAsync(id, patch).ConfigureAwait(true);
        }

        /// <summary>
        /// Posts a new todo item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <response code="201">Item created successfully.</response>
        [HttpPost]
        public async Task<IHttpActionResult> Post(TodoItem item)
        {
            // Assign user id based on calling user
            item.UserId = GetCallerUserIdentifier();

            var current = await InsertAsync(item).ConfigureAwait(true);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        /// <summary>
        /// Deletes the todo item associated with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteTodoItem(string id)
        {
            if (!await ValidateItemIdIsAssociatedWithCallerUserAsync(id).ConfigureAwait(true))
            {
                return Request.CreateNotFoundResponse("Identifier not found or linked with another user");
            }

            await DeleteAsync(id).ConfigureAwait(true);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Initialises the controller
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <inheritdoc />
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<TodoItem>(context, Request, true);
        }

        private async Task<bool> ValidateItemIdIsAssociatedWithCallerUserAsync(string id)
        {
            var item = await LookupAsync(id).ConfigureAwait(false);
            return item.Queryable.FirstOrDefault()?.UserId == GetCallerUserIdentifier();
        }

        private string GetCallerUserIdentifier()
        {
            // Use caller's identity (sub) to locate the bot conversation
            //  associated with this user.
            var user = User as ClaimsPrincipal;
            return user?.Claims?.FirstOrDefault(c => c.Type == "sub")?.Value;
        }
    }
}