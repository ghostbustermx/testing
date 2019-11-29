namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChangeLog", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChangeLog", "Active");
        }
    }
}
