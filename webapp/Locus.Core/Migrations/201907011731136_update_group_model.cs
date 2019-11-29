namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_group_model : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExecutionGroup", "TestEnvironmentId", c => c.Int(nullable: false));
            AddColumn("dbo.ExecutionGroup", "Version", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExecutionGroup", "Version");
            DropColumn("dbo.ExecutionGroup", "TestEnvironmentId");
        }
    }
}
