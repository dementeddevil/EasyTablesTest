namespace Zen.Tracker.Server.Storage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TodoItemUserScoping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TodoItems", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TodoItems", "UserId");
        }
    }
}
