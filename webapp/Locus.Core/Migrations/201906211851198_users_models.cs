namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class users_models : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Division", c => c.String(maxLength: 150));
            AddColumn("dbo.User", "JobTitle", c => c.String(maxLength: 150));
            AddColumn("dbo.User", "Gender", c => c.String(maxLength: 150));
            AddColumn("dbo.User", "Department", c => c.String(maxLength: 150));
            AddColumn("dbo.User", "Alias", c => c.String(maxLength: 150));
            AddColumn("dbo.User", "HireDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "HireDate");
            DropColumn("dbo.User", "Alias");
            DropColumn("dbo.User", "Department");
            DropColumn("dbo.User", "Gender");
            DropColumn("dbo.User", "JobTitle");
            DropColumn("dbo.User", "Division");
        }
    }
}
