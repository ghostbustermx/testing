namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecutionGroupIsReadyProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExecutionGroup", "IsReadyToExecute", c => c.Boolean(nullable: false, defaultValue:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExecutionGroup", "IsReadyToExecute");
        }
    }
}
