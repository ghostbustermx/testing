namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvidenceResultField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResult", "Evidence", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestResult", "Evidence");
        }
    }
}
