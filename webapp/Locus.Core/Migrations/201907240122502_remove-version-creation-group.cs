namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeversioncreationgroup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ExecutionGroup", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExecutionGroup", "Version", c => c.String(nullable: false));
        }
    }
}
