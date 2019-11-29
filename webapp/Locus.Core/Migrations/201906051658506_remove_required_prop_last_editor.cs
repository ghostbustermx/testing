namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_required_prop_last_editor : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TestCase", "Last_Editor", c => c.String(maxLength: 50));
            AlterColumn("dbo.TestProcedure", "Last_Editor", c => c.String(maxLength: 50));
            AlterColumn("dbo.TestScenario", "Last_Editor", c => c.String(nullable: false));
            AlterColumn("dbo.TestSuplemental", "Last_Editor", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestSuplemental", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TestScenario", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TestProcedure", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.TestCase", "Last_Editor", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
