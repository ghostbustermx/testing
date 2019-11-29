namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastEditorField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestCase", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.TestProcedure", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.TestScenario", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.TestSuplemental", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestSuplemental", "Last_Editor");
            DropColumn("dbo.TestScenario", "Last_Editor");
            DropColumn("dbo.TestProcedure", "Last_Editor");
            DropColumn("dbo.TestCase", "Last_Editor");
        }
    }
}
