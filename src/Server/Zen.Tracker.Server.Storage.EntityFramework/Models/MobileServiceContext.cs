﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server.Tables;
using Zen.Tracker.Server.Storage.Entities;

namespace Zen.Tracker.Server.Storage.Models
{
    public class MobileServiceContext : DbContext
    {
#if DEBUG
        private const string ConnectionStringName =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ZenTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
#else
        private const string ConnectionStringName =
            "Name=MS_TableConnectionString";
#endif

        public MobileServiceContext() : base(ConnectionStringName)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        public DbSet<UserConversation> UserConversations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*var schema = ServiceSettingsDictionary.GetSchemaName();
            if (!string.IsNullOrEmpty(schema))
            {
                modelBuilder.HasDefaultSchema(schema);
            }*/

            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }
}
