using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.WindowsAzure.MobileServices;
using Zen.Tracker.Client.Entities;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace Zen.Tracker.Client.Services
{
    public partial class TodoItemManager
    {
#if OFFLINE_SYNC_ENABLED
        private readonly IMobileServiceSyncTable<TodoItem> _todoTable;
#else
        private readonly IMobileServiceTable<TodoItem> _todoTable;
#endif

        private TodoItemManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            CurrentClient.SyncContext.InitializeAsync(store);
            _todoTable = CurrentClient.GetSyncTable<TodoItem>();
#else
            _todoTable = CurrentClient.GetTable<TodoItem>();
#endif
        }

        public static TodoItemManager DefaultManager { get; private set; } = new TodoItemManager();

        public MobileServiceClient CurrentClient { get; }

        public bool IsOfflineEnabled => _todoTable is IMobileServiceSyncTable<TodoItem>;

        public async Task<ObservableCollection<TodoItem>> GetTodoItemsAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await SyncAsync().ConfigureAwait(false);
                }
#endif

                IEnumerable<TodoItem> items = await _todoTable
                    .Where(todoItem => !todoItem.Complete)
                    .ToEnumerableAsync()
                    .ConfigureAwait(false);

                return new ObservableCollection<TodoItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }

            return null;
        }

        public async Task SaveTaskAsync(TodoItem item)
        {
            if (item.Id == null)
            {
                await _todoTable.InsertAsync(item).ConfigureAwait(false);
            }
            else
            {
                await _todoTable.UpdateAsync(item).ConfigureAwait(false);
            }
        }

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.CurrentClient.SyncContext.PushAsync().ConfigureAwait(false);
                await _todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allTodoItems",
                    _todoTable.CreateQuery()).ConfigureAwait(false);
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result).ConfigureAwait(false);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync().ConfigureAwait(false);
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    }
}
