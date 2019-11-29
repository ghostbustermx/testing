namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_expected_result : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TestScenario", "Expected_Result");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestScenario", "Expected_Result", c => c.String(nullable: false));
        }
    }
}
