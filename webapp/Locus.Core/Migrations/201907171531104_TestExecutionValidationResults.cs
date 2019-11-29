namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestExecutionValidationResults : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestExecution", "HasResultsCreated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestExecution", "HasResultsCreated");
        }
    }
}
