using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using AppServiceHelpers.Abstractions;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.Services
{
    public class TodoItemManager : ITodoItemManager
    {
        public TodoItemManager(IEasyMobileServiceClient mobileServiceClient)
        {
            TodoItemTable = mobileServiceClient.Table<TodoItem>();
        }

        private ITableDataStore<TodoItem> TodoItemTable { get; }

        public async Task<ObservableCollection<TodoItem>> GetTodoItemsAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await SyncAsync().ConfigureAwait(false);
                }

                var items = await TodoItemTable
                    .WhereAsync(todoItem => !todoItem.Complete)
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
                await TodoItemTable.AddAsync(item).ConfigureAwait(false);
            }
            else
            {
                await TodoItemTable.UpdateAsync(item).ConfigureAwait(false);
            }
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await TodoItemTable.Sync().ConfigureAwait(false);
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
    }
}
