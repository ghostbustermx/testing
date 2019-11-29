namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Type_Test_Evidence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestCase", "Type", c => c.String(nullable: false, defaultValue: "Functional"));
            AddColumn("dbo.TestProcedure", "Type", c => c.String(nullable: false, defaultValue: "Functional"));
            AddColumn("dbo.TestScenario", "Type", c => c.String(nullable: false, defaultValue: "Functional"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestScenario", "Type");
            DropColumn("dbo.TestProcedure", "Type");
            DropColumn("dbo.TestCase", "Type");
        }
    }
}
