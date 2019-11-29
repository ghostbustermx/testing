namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preconditions1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TestScenario", "Preconditions", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestScenario", "Preconditions", c => c.String(nullable: false));
        }
    }
}
