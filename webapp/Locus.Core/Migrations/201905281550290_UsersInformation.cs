namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersInformation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "FirstName", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.User", "LastName", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.User", "Email", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.User", "PhotoUrl", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.User", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "IsActive");
            DropColumn("dbo.User", "PhotoUrl");
            DropColumn("dbo.User", "Email");
            DropColumn("dbo.User", "LastName");
            DropColumn("dbo.User", "FirstName");
        }
    }
}
