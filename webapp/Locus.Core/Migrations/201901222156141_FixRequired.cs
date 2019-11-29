namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Requirement", "Developer_Assigned", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Requirement", "Tester_Assigned", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Requirement", "Tester_Assigned", c => c.String(maxLength: 50));
            AlterColumn("dbo.Requirement", "Developer_Assigned", c => c.String(maxLength: 50));
        }
    }
}
