using System.Data.Entity;
using Zen.Tracker.Server.Storage.Migrations;
using Zen.Tracker.Server.Storage.Models;

namespace Zen.Tracker.Server.Storage
{
    public class MobileServiceInitializer : MigrateDatabaseToLatestVersion<MobileServiceContext, Configuration>
    {
    }
}