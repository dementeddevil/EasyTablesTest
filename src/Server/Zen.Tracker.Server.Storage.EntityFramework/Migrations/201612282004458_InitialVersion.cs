namespace Zen.Tracker.Server.Storage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TodoItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Version = c.Binary(),
                        CreatedAt = c.DateTimeOffset(precision: 7),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                        Title = c.String(),
                        Text = c.String(),
                        Complete = c.Boolean(nullable: false),
                        DueAt = c.DateTimeOffset(precision: 7),
                        CompletedAt = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserConversations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        ConversationId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserConversations");
            DropTable("dbo.TodoItems");
        }
    }
}
