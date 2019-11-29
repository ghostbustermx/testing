namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_required_prop_last_editor_tp_ts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TestScenario", "Last_Editor", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestScenario", "Last_Editor", c => c.String(nullable: false));
        }
    }
}
